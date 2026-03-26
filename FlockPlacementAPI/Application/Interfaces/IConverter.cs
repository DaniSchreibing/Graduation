using Domain.Data;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
	public interface IConverter
	{
		List<PlacementLine> ConvertPlacements(PyObject pyObject);
		List<PlacementLine> ConvertPlacements2(PyObject pyObject);
		List<KPI> ConvertKPIReport(PyObject pyObject);
		PyObject ConvertPlacements(List<PlacementLine> placements);
		PyObject ConvertOutageScenario(OutageScenario scenario);
	}
}
