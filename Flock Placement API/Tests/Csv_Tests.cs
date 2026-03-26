using Application.Interfaces;
using Application.Use_cases;
using Domain;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
	public class Csv_Tests
	{
		// ✅ HAPPY FLOW
		[Fact]

		public async Task CopyInputData_ShouldCreateFiles()
		{
			var temp = Directory.CreateTempSubdirectory();
			var paths = new Mock<IPaths>();

			paths.Setup(p => p.GetPathItem())
				 .Returns(new PathItem { InputFilePath = temp.FullName });

			var sut = new Csv(paths.Object);

			var mockFile = new Mock<IFormFile>();
			mockFile.Setup(f => f.FileName).Returns("test.csv");
			mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
					.Returns(Task.CompletedTask);

			await sut.CopyInputData_FromList(new List<IFormFile> { mockFile.Object }, Guid.NewGuid());
		}

		// ✅ UNHAPPY FLOW – missing output file
		[Fact]
		public void IsOutputCSVReady_ShouldReturnFalseWhenMissing()
		{
			var temp = Directory.CreateTempSubdirectory();
			var paths = new Mock<IPaths>();

			paths.Setup(p => p.GetPathItem())
				 .Returns(new PathItem { OutputFilePath = temp.FullName });

			var sut = new Csv(paths.Object);

			Assert.False(sut.IsOutputCSVReady(Guid.NewGuid()));
		}

		// ✅ UNHAPPY FLOW – GetOutputCSV throws when file missing
		[Fact]
		public void GetOutputCSV_ShouldThrow_WhenNoFileFound()
		{
			var temp = Directory.CreateTempSubdirectory();
			var paths = new Mock<IPaths>();

			paths.Setup(p => p.GetPathItem())
				 .Returns(new PathItem { OutputFilePath = temp.FullName });

			var sut = new Csv(paths.Object);

			Assert.Throws<ArgumentException>(() => sut.GetOutputCSV(Guid.NewGuid()));
		}

	}
}
