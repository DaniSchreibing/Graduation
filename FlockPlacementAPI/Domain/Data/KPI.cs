using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Data
{
	public class KPI
	{
		public string Name { get; set; }
		public string Scope { get; set; } 
		public double Score { get; set; }
		public string? Description { get; set; }
	}
}
