using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Data
{
	public class StatusItem
	{
		public Guid Guid { get; set; }
		public required string Status { get; set; }
	}
}
