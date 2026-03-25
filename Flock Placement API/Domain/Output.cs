using Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
	public class Output
	{
		public Guid Guid { get; set; }
		public List<KPI> KPIs { get; set; }
	}
}