using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
	public interface IValidate
	{
		bool IsFormulaValid(string formula);
	}
}
