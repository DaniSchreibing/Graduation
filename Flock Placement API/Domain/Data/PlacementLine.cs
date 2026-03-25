using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Data
{
	public class PlacementLine
	{
		public string FlockId { get; set; }
		public string? BarnId { get; set; }
		public DateTime? PlannedStart { get; set; }
		public int AllocatedQty { get; set; }
		public string? ReasonUnplaced { get; set; }
	}
}
