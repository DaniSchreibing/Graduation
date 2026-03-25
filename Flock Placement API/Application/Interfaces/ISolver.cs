using Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
	public interface ISolver
	{
		bool CreatePlacement(Guid guid, out string errorMessage, out dynamic placements2);
		bool CachedReplanOutage(Guid guid, List<PlacementLine> originalPlacements, OutageScenario outageScenario, out string errorMessage, out dynamic placements);
		bool ReplanOutage(Guid guid, OutageScenario outageScenario, out string errorMessage, out dynamic placements);
		bool CreatePlacementWithOutage(Guid guid, out string errorMessage);
		bool GenerateKPIReport(Guid guid, List<PlacementLine> placements, out string errorMessage, out List<KPI> KpiReport);
	}
}
