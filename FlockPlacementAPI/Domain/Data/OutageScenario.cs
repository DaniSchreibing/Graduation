using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Data
{
	public class OutageScenario
	{
		public required string BarnID { get; set; }
		public required DateOnly OutageStart { get; set; }
		public required DateOnly OutageEnd { get; set; }
	}
}
