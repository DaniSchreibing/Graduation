using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
	public class Config
	{
		public bool EnableCaching { get; set; } = true;
		public required PathItem paths { get; set; }
	}
}
