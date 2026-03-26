using Application.Interfaces;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Application.Use_cases
{
	public class Reader : IReaderUC
	{
		public CsvReader CsvReader(string path)
		{
			var reader = new StreamReader(path);
			var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
			return csv;
		}
	}
}
