using Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
	public class Settings
	{
		public required List<Factors> FactorsList { get; set; }
		public required List<KPI> KPIList { get; set; }
	}
}
