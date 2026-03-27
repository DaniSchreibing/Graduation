using Application.Interfaces;
using Application.Use_cases;
using Domain;
using Domain.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Flock_Placement_API.Controllers
{
	[Route("api/FlockPlacement")]
	[ApiController]
	public class FlockPlacementController(IInputValidation InputValidation, ICsv Csv, ISolver Solver, IMemoryCache memoryCache, IStatus Status) : ControllerBase
	{
		[EnableCors("_allowedHosts")]
		[HttpPost("CreatePlacement")]
		public async Task<IActionResult> CreatePlacement(List<IFormFile> files)
		{
			if (!InputValidation.DoesInputHaveFiles(files))
			{
				return BadRequest("No files were uploaded.");
			}

			if (!InputValidation.DoFilesHaveData(files, out var emptyFiles))
			{
				return BadRequest($"The following files are empty: {string.Join(", ", emptyFiles)}");
			}

			var guid = Guid.NewGuid();

			await Csv.CopyInputData_FromList(files, guid);

			var guid2 = guid.ToString();

			try
			{
				return Created(string.Empty, $"Process has initiated with GUID: {guid}");
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex.Message);
				return StatusCode(500, "An error occurred while initiating the placement calculation.");
			}
			finally
			{
				Response.OnCompleted(async () =>
				{
					Solver.CreatePlacement(guid, out var errorMessage, out var placements);

					List<PlacementLine> placementLines = placements;

					Console.WriteLine($"Placement calculation completed for GUID: {guid}");

					memoryCache.Set(
								guid,
								placementLines,
								new MemoryCacheEntryOptions
								{
									AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
								});
				});
			}
		}

		//THIS IS USED/IMPLEMENTED!
		[EnableCors("_allowedHosts")]
		[HttpPost("CreatePlacements/{Guid}")]
		public IActionResult CreatePlacements(Guid Guid)
		{
			Status.WriteStatusItem(Guid);

			if (Solver.CreatePlacement(Guid, out var errorMessage, out var placements) && Csv.IsOutputCSVReady(Guid))
			{
				Status.UpdateStatusItem(Guid);
				return Created();
			}
			if (!Csv.IsOutputCSVReady(Guid))
			{
				Response.Headers.Append("Message", "Placement is not ready!");
				return NotFound();
			}
			if (errorMessage != null)
			{
				Response.Headers.Append("Message", errorMessage);
				return NotFound();
			}
			return Problem("There was a problem whilst creating the placements");
		}

		//THIS IS USED/IMPLEMENTED
		[EnableCors("_allowedHosts")]
		[HttpGet("GetStatus/{Guid}/{CompanyName}")]
		public IActionResult GetStatus(Guid Guid, string CompanyName)
		{
			if (!Status.FindStatusItem(Guid))
			{
				Console.WriteLine($"No status item with id: {Guid} could be found");
				return NotFound($"No status item with id: {Guid} could be found");
			}
			if (!Status.IsStatusReady(Guid))
			{
				Response.Headers.Append("Message", "not ready");
				return Ok("not ready");
			}
			Response.Headers.Append("Message", "ready");
			return Ok("ready");
		}

		//THIS IS USED/IMPLEMENTED
		[EnableCors("_allowedHosts")]
		[HttpPost("CreateCSV/{fileName}/{guid}/{CompanyName}")]
		public async Task<IActionResult> CreateCSVFiles(string fileName, Guid guid, string CompanyName)
		{
			Console.WriteLine($"Received request to create CSV file: {fileName} for GUID: {guid} and Company: {CompanyName}");

			var data = await new StreamReader(Request.Body).ReadToEndAsync();

			Console.WriteLine(data);

			await Csv.CopyInputData(data, fileName, guid);

			return Ok();
		}

		//THIS IS USED/IMPLEMENTED
		[EnableCors("_allowedHosts")]
		[HttpGet("DownloadPlacementCSV/{guid}")]
		public IActionResult DownloadPlacementCSV(Guid guid)
		{
			if (!Status.IsStatusReady(guid) || !Csv.IsOutputCSVReady(guid))
			{
				return NotFound($"Placement CSV is not ready for GUID: {guid}.");
			}

			var fileContentResult = Csv.GetOutputCSV(guid);
			if (fileContentResult != null)
			{
				return fileContentResult;
			}
			return NotFound($"Could not download placement CSV for GUID: {guid}.");
		}

		//THIS IS USED/IMPLEMENTED
		[EnableCors("_allowedHosts")]
		[HttpGet("DownloadKPICSV/{guid}")]
		public IActionResult DownloadKPICSV(Guid guid)
		{
			if (!Csv.IsKPICSVReady(guid))
			{
				return NotFound($"KPI CSV is not ready for GUID: {guid}.");
			}

			var fileContentResult = Csv.GetKPICSV(guid);
			if (fileContentResult != null)
			{
				return fileContentResult;
			}
			return NotFound($"Could not download KPI CSV for GUID: {guid}.");
		}

		[HttpPost("OutageSimulation/{guid}")]
		public IActionResult OutageSimulation(Guid guid, OutageScenario outageScenario)
		{
			dynamic updatedPlacementLines = null;

			if (memoryCache.TryGetValue(guid, out List<PlacementLine>? placementLines))
			{
				Solver.CachedReplanOutage(guid, placementLines, outageScenario, out var errorMessage, out var updatedPlacements2);

				return Ok(updatedPlacements2);
			}
			else if (Solver.ReplanOutage(guid, outageScenario, out var errorMessage2, out var updatedPlacements))
			{
				return Ok(updatedPlacements);
			}

			return NotFound($"No placement found for GUID: {guid}");
		}

		//TODO: Remove this endpoint once testing is complete
		[HttpPost("SetStatus/{guid}")]
		public IActionResult SetStatus(string guid, string status)
		{
			memoryCache.Set(
				guid.ToString(),
				status,
				new MemoryCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
				});
			return Ok($"Status for GUID: {guid} set to {status}.");
		}

		//TODO: Remove this endpoint once testing is complete
		[HttpGet("GetStatus2/{guid}")]
		public IActionResult GetStatus2(Guid guid)
		{
			Console.WriteLine($"Received request to get status for GUID: {guid}..");

			if (memoryCache.TryGetValue(guid, out string? status))
			{
				Console.WriteLine($"Status for GUID: {guid}.");
				return Ok(new { GUID = guid, Status = status });
			}
			else
			{
				return NotFound($"No status found for GUID: {guid}");
			}
		}

		[HttpGet("GetPlacement2/{guid}")]
		public IActionResult GetPlacement2(Guid guid)
		{
			if (memoryCache.TryGetValue(guid, out List<PlacementLine>? placement))
			{
				return Ok(placement);
			}
			else if (Csv.GetPlacementLines(guid, out var placementLines))
			{
				return Ok(placementLines);
			}
			return NotFound($"No placement found for GUID: {guid}");
		}

		[HttpPost("GenerateKPIReport/{guid}")]
		public IActionResult GenerateKPIReport(Guid guid)
		{
			if (!memoryCache.TryGetValue(guid, out List<PlacementLine>? placements) && !Csv.GetPlacementLines2(guid, out placements))
			{
				Console.WriteLine($"Error generating KPI report for GUID: {guid}");
				Response.Headers.Append("Message", "No placement found");
				return NotFound($"No placement found for GUID: {guid}");
			}
			if (Solver.GenerateKPIReport(guid, placements, out var errorMessage, out var Report))
			{
				return Ok(Report);
			}
			Console.WriteLine($"Error generating KPI report for GUID: {guid}. Error message: {errorMessage}");
			Response.Headers.Append("Message", "An error occurred while generating the KPI report:");
			return StatusCode(500, $"An error occurred while generating the KPI report: {errorMessage}");
		}

		[HttpPost]

		[HttpGet("GetKPIReport/{guid}")]
		public IActionResult GetKPIReport(Guid guid)
		{
			if (Csv.GetKPIReport(guid, out var kpiReport))
			{
				return Ok(kpiReport);
			}
			return NotFound($"No KPI report could be found for GUID: {guid}");
		}

		[HttpGet("GetPlacement/{guid}")]
		public IActionResult GetPlacement(Guid guid)
		{
			if (memoryCache.TryGetValue(guid, out List<PlacementLine>? placementLines) || Csv.GetPlacementLines(guid, out placementLines))
			{
				return Ok(placementLines);
			}
			return NotFound($"No placement found for GUID: {guid}");
		}

		[HttpPost("SetConfig")]
		public IActionResult SetConfig(Config config)
		{
			return Ok(config);
		}
	}
}