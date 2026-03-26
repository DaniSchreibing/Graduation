using Domain.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
	public interface ICsv
	{
		Task CopyInputData_FromList(List<IFormFile> data, Guid guid);
		Task CopyInputData(string? data, string fileName, Guid? guid);
		public bool IsOutputCSVReady(Guid id);
		public bool IsKPICSVReady(Guid id);
		public FileContentResult GetOutputCSV(Guid guid);
		public FileContentResult GetKPICSV(Guid guid);
		public bool GetPlacementLines(Guid guid, out List<PlacementLine> placementLines);
		public bool GetPlacementLines2(Guid guid, out List<PlacementLine> placementLines);
		public bool GetKPIReport(Guid guid, out List<KPI> kpiReport);
	}
}
