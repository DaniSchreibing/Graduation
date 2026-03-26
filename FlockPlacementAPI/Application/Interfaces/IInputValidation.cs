using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
	public interface IInputValidation
	{
		bool DoesInputHaveFiles(List<IFormFile> data);
		bool DoFilesHaveData(List<IFormFile> data, out List<string>? emptyFiles);
	}
}
