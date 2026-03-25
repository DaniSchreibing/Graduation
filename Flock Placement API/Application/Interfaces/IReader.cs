using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper;

namespace Application.Interfaces
{
	public interface IReaderUC
	{
		public CsvReader CsvReader(string Path);
	}
}
