using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Use_cases
{
	public class Validate : IValidate
	{
		public bool IsFormulaValid(string formula)
		{
			if (string.IsNullOrWhiteSpace(formula))
			{
				return false;
			}

			var regex = new Regex(@"^[0-9+\-*/().\s]+$");
			if (!regex.IsMatch(formula))
			{
				return false;
			}
			return true;
		}
	}
}
