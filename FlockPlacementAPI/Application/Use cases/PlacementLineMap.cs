using CsvHelper.Configuration;
using Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Use_cases
{
	public sealed class PlacementLineMap : ClassMap<PlacementLine>
	{
		public PlacementLineMap()
		{
			Map(m => m.FlockId).Name("flock_id");
			Map(m => m.BarnId).Name("barn_id");
			Map(m => m.PlannedStart).Name("planned_start");
			Map(m => m.AllocatedQty).Name("allocated_qty");
			Map(m => m.ReasonUnplaced).Name("reason_unplaced");
		}
	}
}
