using Application.Use_cases;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tests
{
	public class Path_Tests
	{

		// ✅ HAPPY FLOW
		[Fact]
		public void GetPathItem_ShouldReturnValidObject()
		{
			var tmp = Path.GetTempFileName();
			File.WriteAllText(tmp, "{\"StatusFilePath\":\"abc.csv\"}");

			var sut = new PathUC();
			typeof(PathUC)
				.GetField("fileName", BindingFlags.NonPublic | BindingFlags.Instance)
				.SetValue(sut, tmp);

			var result = sut.GetPathItem();
			Assert.Equal("abc.csv", result.StatusFilePath);
		}

		// ✅ UNHAPPY FLOW – missing JSON file
		[Fact]
		public void GetPathItem_ShouldThrow_WhenFileMissing()
		{
			var sut = new PathUC();
			typeof(PathUC)
				.GetField("fileName", BindingFlags.NonPublic | BindingFlags.Instance)
				.SetValue(sut, "does_not_exist.json");

			Assert.Throws<FileNotFoundException>(() => sut.GetPathItem());
		}

		// ✅ UNHAPPY FLOW – invalid JSON
		[Fact]
		public void GetPathItem_ShouldThrow_OnInvalidJson()
		{
			var tmp = Path.GetTempFileName();
			File.WriteAllText(tmp, "INVALID_JSON");

			var sut = new PathUC();
			typeof(PathUC)
				.GetField("fileName", BindingFlags.NonPublic | BindingFlags.Instance)
				.SetValue(sut, tmp);

			Assert.ThrowsAny<Exception>(() => sut.GetPathItem());
		}

	}
}
