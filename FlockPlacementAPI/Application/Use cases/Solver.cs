using Application.Interfaces;
using Domain;
using Domain.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.JSInterop;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Xml.XPath;

namespace Application.Use_cases
{
	public class Solver(IPaths Paths, IMemoryCache memoryCache, IConverter Converter, IPaths pathItem) : ISolver
	{
		static Solver()
		{
			Runtime.PythonDLL = @"C:\Users\danis\AppData\Local\Python\pythoncore-3.12-64\python312.dll";
			PythonEngine.Initialize();
			PythonEngine.BeginAllowThreads();
		}

		public bool CreatePlacement(Guid guid, out string errorMessage, out dynamic placements3)
		{
			errorMessage = string.Empty;
			placements3 = new List<PlacementLine>();

			try
			{
				using (Py.GIL())
				{
					dynamic sys = Py.Import("sys");
					sys.path.append(Paths.GetPathItem().PythonCodePath);

					var script = Py.Import("main");
					PyObject pyGuid = guid.ToPython();

					dynamic result = script.InvokeMethod("generate_plan2", pyGuid);
					
					placements3 = Converter.ConvertPlacements2(result);
				}
				return true;
			}
			catch (PythonException ex)
			{
				Console.WriteLine(ex.Message);
				errorMessage = ex.Message;
				return false;
			}
		}

		public bool CreatePlacementWithOutage(Guid id, out string errorMessage)
		{
			throw new NotImplementedException();
		}

		private bool isReplanned(Guid guid)
		{
			var filePathReplanned = Path.Combine($@"{pathItem.GetPathItem().OutputFilePath}\{guid}", "replanned_placement_lines.csv");

			if (File.Exists(filePathReplanned))
			{
				return true;
			}
			return false;
		}

		public bool GenerateKPIReport(Guid guid, List<PlacementLine> placements, out string errorMessage, out List<KPI> KpiReport)
		{
			errorMessage = string.Empty;
			KpiReport = new List<KPI>();

			try
			{
				using (Py.GIL())
				{
					dynamic sys = Py.Import("sys");
					sys.path.append(Paths.GetPathItem().PythonCodePath);

					var script = Py.Import("main");
					PyObject pyGuid = guid.ToPython();
					PyObject pyPlacements = Converter.ConvertPlacements(placements);
					PyObject pyUseReplanned = isReplanned(guid).ToPython();

					dynamic result = script.InvokeMethod("generate_kpi_report", pyGuid, pyPlacements, pyUseReplanned);

					KpiReport = Converter.ConvertKPIReport(result);
				}
				return true;
			}
			catch (PythonException ex)
			{
				Console.WriteLine(ex.Message);
				errorMessage = ex.Message;
				return false;
			}

			throw new NotImplementedException();
		}

		public bool CachedReplanOutage(Guid guid, List<PlacementLine> originalPlacements, OutageScenario outageScenario, out string errorMessage, out dynamic placements)
		{
			errorMessage = string.Empty;
			placements = new List<PlacementLine>();

			try
			{
				using (Py.GIL())
				{
					dynamic sys = Py.Import("sys");
					sys.path.append(Paths.GetPathItem().PythonCodePath);

					var script = Py.Import("main");
					PyObject pyGuid = guid.ToPython();
					PyObject pyOriginalPlacements = Converter.ConvertPlacements(originalPlacements);
					PyObject pyOutageScenario = Converter.ConvertOutageScenario(outageScenario);

					dynamic result = script.InvokeMethod("cached_replan_for_outage", pyGuid, pyOriginalPlacements, pyOutageScenario);

					placements = Converter.ConvertPlacements(result);

					return true;
				}
			}
			catch (PythonException ex)
			{
				Console.WriteLine(ex.Message);
				errorMessage = ex.Message;
				return false;
			}
		}

		public bool ReplanOutage(Guid guid, OutageScenario outageScenario, out string errorMessage, out dynamic placements)
		{
			errorMessage = string.Empty;
			placements = new List<PlacementLine>();

			try
			{
				using (Py.GIL())
				{
					dynamic sys = Py.Import("sys");
					sys.path.append(Paths.GetPathItem().PythonCodePath);

					var script = Py.Import("main");
					PyObject pyGuid = guid.ToPython();
					PyObject pyOutageScenario = Converter.ConvertOutageScenario(outageScenario);

					dynamic result = script.InvokeMethod("replan_for_outage", pyGuid, pyOutageScenario);

					placements = Converter.ConvertPlacements(result);
				}
				return true;
			}
			catch (PythonException ex)
			{
				Console.WriteLine(ex.Message);
				errorMessage = ex.Message;
				return false;
			}
		}
	}
}
