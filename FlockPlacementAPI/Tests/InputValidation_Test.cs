using Application.Use_cases;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
	public class InputValidation_Test
	{
		private readonly InputValidation _validator = new();

		[Fact]
		public void DoesInputHaveFiles_ReturnsFalse_WhenNullOrEmpty()
		{
			Assert.False(_validator.DoesInputHaveFiles(null));
			Assert.False(_validator.DoesInputHaveFiles(new List<IFormFile>()));
		}

		[Fact]
		public void DoesInputHaveFiles_ReturnsTrue_WhenFilesExist()
		{
			var files = new List<IFormFile> { new Mock<IFormFile>().Object };
			Assert.True(_validator.DoesInputHaveFiles(files));
		}

		[Fact]
		public void DoFilesHaveData_ReturnsFalse_AndPopulatesEmptyFiles_WhenLengthIsTwoOrLess()
		{
			var mockFile = new Mock<IFormFile>();
			mockFile.Setup(f => f.Length).Returns(2);
			mockFile.Setup(f => f.FileName).Returns("empty.csv");

			var result = _validator.DoFilesHaveData(new List<IFormFile> { mockFile.Object }, out var emptyFiles);

			Assert.False(result);
			Assert.Single(emptyFiles);
			Assert.Equal("empty.csv", emptyFiles[0]);
		}
	}
}
