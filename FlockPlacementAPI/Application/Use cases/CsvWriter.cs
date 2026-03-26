using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CsvHelper;

namespace Application.Use_cases
{
	public static class WriterUC
	{
		public static CsvWriter Writer(string path, bool append)
		{
			var writer = new StreamWriter(path, append: append);
			var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
			return csv;
		}
	}
}
