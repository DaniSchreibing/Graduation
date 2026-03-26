using Application.Use_cases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
	public class CsvWriter_Tests
	{

		// ✅ HAPPY FLOW
		[Fact]
		public void Writer_ShouldReturnCsvWriter()
		{
			var temp = Path.GetTempFileName();
			var writer = WriterUC.Writer(temp, false);

			Assert.NotNull(writer);
		}

		// ✅ UNHAPPY FLOW
		[Fact]
		public void Writer_ShouldThrow_OnInvalidPath()
		{
			Assert.ThrowsAny<Exception>(() =>
				WriterUC.Writer("???/invalid/path.csv", false)
			);
		}

	}
}
