using CsvHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
	public interface ICsvWriter
	{
		CsvWriter Writer(string path, bool append);
	}
}
