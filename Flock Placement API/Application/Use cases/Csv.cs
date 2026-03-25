using Application.Interfaces;
using CsvHelper;
using Domain;
using Domain.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.XPath;

namespace Application.Use_cases
{
	public class Csv(IPaths pathItem) : ICsv
	{
		public async Task CopyInputData_FromList(List<IFormFile> data, Guid guid)
		{
			Directory.CreateDirectory($@"{pathItem.GetPathItem().InputFilePath}\{guid}");

			foreach (var file in data)
			{
				var filePath = Path.Combine($@"{pathItem.GetPathItem().InputFilePath}\{guid}", file.FileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}
			}
		}

		public async Task CopyInputData(string? data, string fileName, Guid? guid)
		{
			Directory.CreateDirectory($@"{pathItem.GetPathItem().InputFilePath}\{guid}");

			var filePath = Path.Combine($@"{pathItem.GetPathItem().InputFilePath}\{guid}", $"{fileName}.csv");

			Console.WriteLine(filePath);

			await File.WriteAllTextAsync(filePath, data);
		}

		public async Task SaveConfig(Config config)
		{
			if (config == null)
			{
				throw new ArgumentNullException(nameof(config));
			}

			
		}

		public bool IsOutputCSVReady(Guid id)
		{
			return (File.Exists(@$"{pathItem.GetPathItem().OutputFilePath}\{id}\placement_lines.csv"));
		}

		public bool IsKPICSVReady(Guid id)
		{
			return (File.Exists(@$"{pathItem.GetPathItem().OutputFilePath}\{id}\kpi_results.csv"));
		}

		public FileContentResult GetOutputCSV(Guid guid)
		{
			var filePath = String.Empty;
			var filePathOriginal = Path.Combine($@"{pathItem.GetPathItem().OutputFilePath}\{guid}", "placement_lines.csv");
			var filePathReplanned = Path.Combine($@"{pathItem.GetPathItem().OutputFilePath}\{guid}", "replanned_placement_lines.csv");

			if (File.Exists(filePathReplanned))
			{
				filePath = filePathReplanned;
			}
			else if (File.Exists(filePathOriginal))
			{
				filePath = filePathOriginal;
			}

			byte[] fileBytes = File.ReadAllBytes(filePath);

			return new FileContentResult(fileBytes, "text/csv")
			{
				FileDownloadName = "placement_lines.csv"
			};
		}

		public FileContentResult GetKPICSV(Guid guid)
		{
			var filePath = Path.Combine($@"{pathItem.GetPathItem().OutputFilePath}\{guid}", "kpi_results.csv");

			byte[] fileBytes = File.ReadAllBytes(filePath);

			return new FileContentResult(fileBytes, "text/csv")
			{
				FileDownloadName = "kpi_results.csv"
			};
		}

		public bool GetPlacementLines(Guid guid, out List<PlacementLine> placementLines)
		{
			placementLines = new List<PlacementLine>();
			
			var filePath = String.Empty;
			var filePathOriginal = Path.Combine($@"{pathItem.GetPathItem().OutputFilePath}\{guid}", "placement_lines.csv");
			var filePathReplanned = Path.Combine($@"{pathItem.GetPathItem().OutputFilePath}\{guid}", "replanned_placement_lines.csv");

			if (File.Exists(filePathReplanned))
			{
				filePath = filePathReplanned;
			}
			else if (File.Exists(filePathOriginal))
			{
				filePath = filePathOriginal;
			}
			else
			{
				return false;
			}

			var lines = File.ReadAllLines(filePath);

			for (int i = 1; i < lines.Length; i++)
			{
				var columns = lines[i].Split(',');
				var placementLine = new PlacementLine
				{
					FlockId = columns[0],
					BarnId = columns[1],
					PlannedStart = DateTime.Parse(columns[2]),
					AllocatedQty = int.Parse(columns[3]),
					ReasonUnplaced = columns[4]
				};
				placementLines.Add(placementLine);
			}
			return true;
		}

		public bool GetPlacementLines2(Guid guid, out List<PlacementLine> placementLines)
		{
			placementLines = new List<PlacementLine>();

			var filePath = string.Empty;
			var filePathOriginal = Path.Combine($@"{pathItem.GetPathItem().OutputFilePath}\{guid}", "placement_lines.csv");
			var filePathReplanned = Path.Combine($@"{pathItem.GetPathItem().OutputFilePath}\{guid}", "replanned_placement_lines.csv");

			if (File.Exists(filePathReplanned))
			{
				filePath = filePathReplanned;
			}
			else if (File.Exists(filePathOriginal))
			{
				filePath = filePathOriginal;
			}
			else
			{
				return false;
			}

			using var reader = new StreamReader(filePath);
			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

			csv.Context.RegisterClassMap<PlacementLineMap>();

			placementLines = csv.GetRecords<PlacementLine>().ToList();

			return true;
		}

		public bool GetKPIReport(Guid guid, out List<KPI> kpiReport)
		{
			kpiReport = new List<KPI>();

			var filePathKpiReport = Path.Combine($@"{pathItem.GetPathItem().OutputFilePath}\{guid}", "kpi_results.csv");
			if (!File.Exists(filePathKpiReport))
			{
				return false;
			}

			var lines = File.ReadAllLines(filePathKpiReport);

			for (int i = 1; i < lines.Length; i++)
			{
				var columns = lines[i].Split(',');
				var kpi = new KPI
				{
					Name = columns[0],
					Scope = columns[1],
					Score = double.Parse(columns[2]),
					Description = columns.Length > 3 ? columns[3] : null,
				};
				kpiReport.Add(kpi);
			}
			return true;
		}
	}
}
