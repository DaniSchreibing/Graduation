using Application.Use_cases;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
	public class InputValidation_Tests
	{

		// ✅ HAPPY FLOW
		[Fact]
		public void DoesInputHaveFiles_ShouldReturnTrue_WhenFilesPresent()
		{
			var sut = new InputValidation();
			var list = new List<IFormFile> { Mock.Of<IFormFile>() };

			Assert.True(sut.DoesInputHaveFiles(list));
		}

		// ✅ UNHAPPY FLOW – null
		[Fact]
		public void DoesInputHaveFiles_ShouldReturnFalse_WhenNull()
		{
			var sut = new InputValidation();
			Assert.False(sut.DoesInputHaveFiles(null));
		}

		// ✅ UNHAPPY FLOW – empty
		[Fact]
		public void DoesInputHaveFiles_ShouldReturnFalse_WhenEmpty()
		{
			var sut = new InputValidation();
			Assert.False(sut.DoesInputHaveFiles(new List<IFormFile>()));
		}

		// ✅ HAPPY FLOW – all files have content
		[Fact]
		public void DoFilesHaveData_ShouldReturnTrue_WhenAllHaveData()
		{
			var file = new Mock<IFormFile>();
			file.Setup(f => f.Length).Returns(10);
			file.Setup(f => f.FileName).Returns("test.csv");

			var sut = new InputValidation();
			var result = sut.DoFilesHaveData(new List<IFormFile> { file.Object }, out var empty);

			Assert.True(result);
			Assert.Empty(empty);
		}

		// ✅ UNHAPPY FLOW – file empty
		[Fact]
		public void DoFilesHaveData_ShouldDetectEmptyFiles()
		{
			var file = new Mock<IFormFile>();
			file.Setup(f => f.Length).Returns(0);
			file.Setup(f => f.FileName).Returns("empty.csv");

			var sut = new InputValidation();
			var result = sut.DoFilesHaveData(new List<IFormFile> { file.Object }, out var empty);

			Assert.False(result);
			Assert.Single(empty);
			Assert.Equal("empty.csv", empty[0]);
		}

	}
}
