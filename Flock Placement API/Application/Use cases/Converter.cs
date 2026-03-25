using Application.Interfaces;
using Domain.Data;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Use_cases
{
	public class Converter: IConverter
	{
		public List<PlacementLine> ConvertPlacements(PyObject pyObject)
		{
			var placements = new List<PlacementLine>();

			using var pyList = new PyList(pyObject);

			foreach (PyObject pyPlacement in pyList)
			{
				var placement = new PlacementLine
				{
					FlockId = pyPlacement.GetAttr("flock_id").ToString(),
					BarnId = pyPlacement.GetAttr("barn_id").ToString(),
					PlannedStart = DateTime.Parse(pyPlacement.GetAttr("planned_start").ToString()),
					AllocatedQty = int.Parse(pyPlacement.GetAttr("allocated_qty").ToString()),
					ReasonUnplaced = pyPlacement.GetAttr("reason_unplaced").ToString()
				};
				placements.Add(placement);
			}
			return placements;
		}

		public List<PlacementLine> ConvertPlacements2(PyObject pyObject)
		{
			var placements = new List<PlacementLine>();

			using var pyList = new PyList(pyObject);

			foreach (PyObject pyPlacement in pyList)
			{
				string flockId = pyPlacement.GetAttr("flock_id")?.ToString();
				string barnId = pyPlacement.GetAttr("barn_id")?.ToString();
				string reasonUnplaced = pyPlacement.GetAttr("reason_unplaced")?.ToString();

				DateTime? plannedStart = null;
				var plannedStartObj = pyPlacement.GetAttr("planned_start");
				if (plannedStartObj != null && plannedStartObj.ToString() != "None")
				{
					DateTime.TryParse(plannedStartObj.ToString(), out DateTime parsedDate);
					plannedStart = parsedDate;
				}

				int allocatedQty = 0;
				var qtyObj = pyPlacement.GetAttr("allocated_qty");
				if (qtyObj != null)
				{
					int.TryParse(qtyObj.ToString(), out allocatedQty);
				}

				placements.Add(new PlacementLine
				{
					FlockId = flockId,
					BarnId = barnId,
					PlannedStart = plannedStart,
					AllocatedQty = allocatedQty,
					ReasonUnplaced = reasonUnplaced
				});
			}

			return placements;
		}

		public List<KPI> ConvertKPIReport(PyObject pyObject)
		{
			var kpiReport = new List<KPI>();

			using var pyList = new PyList(pyObject);

			foreach (PyObject pyKPI in pyList)
			{
				var kpi = new KPI
				{
					Name = pyKPI.GetAttr("name").ToString(),
					Score = pyKPI.GetAttr("value").ToDouble(System.Globalization.CultureInfo.InvariantCulture),
					Description = pyKPI.GetAttr("description").ToString(),
				};
				kpiReport.Add(kpi);
			}
			return kpiReport;
		}

		public PyObject ConvertPlacements(List<PlacementLine> placements)
		{
			var pyList = new PyList();

			foreach (var placement in placements)
			{
				var pyPlacement = new PyDict();
				pyPlacement.SetItem("flock_id", placement.FlockId.ToPython());
				pyPlacement.SetItem("barn_id", placement.BarnId.ToPython());
				pyPlacement.SetItem("planned_start", placement.PlannedStart.HasValue ? placement.PlannedStart.Value.ToString("yyyy-MM-dd").ToPython() : PyObject.None);
				pyPlacement.SetItem("allocated_qty", placement.AllocatedQty.ToPython());
				pyPlacement.SetItem("reason_unplaced", placement.ReasonUnplaced.ToPython());
				pyList.Append(pyPlacement);
				pyPlacement.Dispose();
			}
			return pyList;
		}

		public PyObject ConvertOutageScenario(OutageScenario scenario)
		{
			var pyDict = new PyDict();
			pyDict.SetItem("barn_id", scenario.BarnID.ToPython());
			pyDict.SetItem("outage_start", scenario.OutageStart.ToPython());
			pyDict.SetItem("outage_end", scenario.OutageEnd.ToPython());
			return pyDict;
		}
	}
}
