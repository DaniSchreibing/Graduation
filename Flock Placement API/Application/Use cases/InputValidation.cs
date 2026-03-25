using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Use_cases
{
	public class InputValidation : IInputValidation
	{
		public bool DoesInputHaveFiles(List<IFormFile> data)
		{
			return data != null && data.Count > 0;
		}

		public bool DoFilesHaveData(List<IFormFile> data, out List<string>? emptyFiles)
		{
			emptyFiles = new List<string>();

			foreach (var file in data)
			{
				if (file.Length <= 2)
				{
					emptyFiles.Add(file.FileName);
				}
			}

			return emptyFiles.Count == 0;
		}
	}
}
