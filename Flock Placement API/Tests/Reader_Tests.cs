using Application.Use_cases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
	public class Reader_Tests
	{

		// ✅ HAPPY FLOW
		[Fact]
		public void CsvReader_ShouldReturnValidReader()
		{
			var temp = Path.GetTempFileName();
			File.WriteAllText(temp, "A,B");

			var reader = new Reader();
			var csv = reader.CsvReader(temp);

			Assert.NotNull(csv);
		}

		// ✅ UNHAPPY FLOW
		[Fact]
		public void CsvReader_ShouldThrow_OnMissingFile()
		{
			var sut = new Reader();
			Assert.Throws<FileNotFoundException>(() => sut.CsvReader("missing.csv"));
		}

	}
}
