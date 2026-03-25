using Application.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Application.Use_cases
{
	public class PathUC : IPaths
	{
		string fileName = @"C:\Users\DaniS\Documents\FlockPlanner\API_Data\Config.json";

		public PathItem GetPathItem()
		{
			string jsonString = File.ReadAllText(fileName);
			PathItem paths = JsonSerializer.Deserialize<PathItem>(jsonString)!;
			return paths;
		}
	}
}
