using Application.Use_cases;
using Domain.Data;
using Python.Runtime;
using Xunit;

namespace Tests
{
	public class ConverterTests
	{
		private readonly Converter _converter = new Converter();

		public ConverterTests()
		{
			PythonEngine.Initialize();
		}

		[Fact]
		public void ConvertPlacements2_ShouldConvertPythonObjects_ToPlacementLines()
		{
			using (Py.GIL())
			{
				dynamic placement = new PyDict();
				placement["flock_id"] = "F1";
				placement["barn_id"] = "B1";
				placement["planned_start"] = "2025-01-01";
				placement["allocated_qty"] = "100";
				placement["reason_unplaced"] = "None";

				var pyList = new PyList();
				pyList.Append(placement);

				var result = _converter.ConvertPlacements2(pyList);

				Assert.Single(result);
				Assert.Equal("F1", result[0].FlockId);
				Assert.Equal("B1", result[0].BarnId);
				Assert.Equal(100, result[0].AllocatedQty);
				Assert.Equal(new DateTime(2025, 1, 1), result[0].PlannedStart);
			}
		}

		[Fact]
		public void ConvertPlacements_ShouldConvertPlacementLines_ToPythonList()
		{
			using (Py.GIL())
			{
				var placements = new List<PlacementLine>
				{
					new PlacementLine
					{
						FlockId = "F1",
						BarnId = "B1",
						PlannedStart = new DateTime(2025,1,1),
						AllocatedQty = 200,
						ReasonUnplaced = null
					}
				};

				var result = _converter.ConvertPlacements(placements);

				var pyList = new PyList(result);
				Assert.Equal(1, pyList.Length());
			}
		}

		[Fact]
		public void ConvertKPIReport_ShouldConvertPythonObjects_ToKPIList()
		{
			using (Py.GIL())
			{
				dynamic pyKpi = new PyDict();
				pyKpi["name"] = "Utilization";
				pyKpi["value"] = 0.95;
				pyKpi["description"] = "Barn utilization";

				var pyList = new PyList();
				pyList.Append(pyKpi);

				var result = _converter.ConvertKPIReport(pyList);

				Assert.Single(result);
				Assert.Equal("Utilization", result[0].Name);
				Assert.Equal(0.95, result[0].Score);
				Assert.Equal("Barn utilization", result[0].Description);
			}
		}
	}
}