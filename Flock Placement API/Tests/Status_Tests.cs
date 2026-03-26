using Application.Interfaces;
using Application.Use_cases;
using CsvHelper;
using Domain;
using Domain.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Tests
{
	public class Status_Tests
	{
		private string CreateCsv(params StatusItem[] items)
		{
			var path = Path.GetTempFileName();
			using var writer = new StreamWriter(path);
			writer.WriteLine("Guid,Status");

			foreach (var i in items)
				writer.WriteLine($"{i.Guid},{i.Status}");

			return path;
		}

		// ✅ HAPPY FLOW
		[Fact]
		public void FindStatusItem_ShouldReturnTrue_WhenGuidExists()
		{
			var id = Guid.NewGuid();
			var path = CreateCsv(new StatusItem { Guid = id, Status = "ok" });

			var paths = new Mock<IPaths>();
			paths.Setup(p => p.GetPathItem())
				 .Returns(new PathItem { StatusFilePath = path });

			var reader = new Mock<IReaderUC>();
			reader.Setup(r => r.CsvReader(path))
				  .Returns(new CsvReader(new StreamReader(path), CultureInfo.InvariantCulture));

			var sut = new Status(paths.Object, reader.Object);
			Assert.True(sut.FindStatusItem(id));
		}

		// ✅ UNHAPPY FLOW – no match
		[Fact]
		public void FindStatusItem_ShouldReturnFalse_WhenNotFound()
		{
			var id = Guid.NewGuid();
			var path = CreateCsv();

			var paths = new Mock<IPaths>();
			paths.Setup(p => p.GetPathItem())
				 .Returns(new PathItem { StatusFilePath = path });

			var reader = new Mock<IReaderUC>();
			reader.Setup(r => r.CsvReader(path))
				  .Returns(new CsvReader(new StreamReader(path), CultureInfo.InvariantCulture));

			var sut = new Status(paths.Object, reader.Object);
			Assert.False(sut.FindStatusItem(id));
		}
	}
}
