using Application.Interfaces;
using Domain;
using Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.IO;
using System.Linq;

namespace Application.Use_cases
{
	public class Status(IPaths Paths, IReaderUC Reader) : IStatus
	{
		// Ensure the CSV file exists and contains header names matching StatusItem properties.
		private void EnsureStatusFileHeader()
		{
			var path = Paths.GetPathItem().StatusFilePath;
			// Ensure directory exists
			var dir = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			// Prepare header line from StatusItem property names
			var header = string.Join(",", typeof(StatusItem).GetProperties().Select(p => p.Name));

			// If file doesn't exist or is empty, write header
			if (!File.Exists(path) || new FileInfo(path).Length == 0)
			{
				File.WriteAllText(path, header + Environment.NewLine);
				return;
			}

			// If file exists, check first line for required headers
			string firstLine;
			using (var sr = new StreamReader(path))
			{
				firstLine = sr.ReadLine() ?? string.Empty;
			}

			// If header is missing (simple check: doesn't contain all property names), prepend header
			var requiredNames = typeof(StatusItem).GetProperties().Select(p => p.Name);
			bool hasAll = requiredNames.All(name => firstLine.Contains(name, StringComparison.OrdinalIgnoreCase));

			if (!hasAll)
			{
				var existing = File.ReadAllText(path);
				File.WriteAllText(path, header + Environment.NewLine + existing);
			}
		}

		public bool FindStatusItem(Guid Guid)
		{
			EnsureStatusFileHeader();

			using var csv = Reader.CsvReader(Paths.GetPathItem().StatusFilePath);

			var tests = csv.GetRecords<StatusItem>();
			foreach (var test in tests
						 .Where(t => t.Guid.Equals(Guid)))
			{
				return true;
			}
			return false;
		}

		public bool IsStatusReady(Guid Guid)
		{
			EnsureStatusFileHeader();

			using var csv = Reader.CsvReader(Paths.GetPathItem().StatusFilePath);

			var tests = csv.GetRecords<StatusItem>();
			foreach (var test in tests
						 .Where(t => t.Guid.Equals(Guid))
						 .Where(t => t.Status.Equals("ready")))
			{
				return true;
			}
			return false;
		}

		public void UpdateStatusItem(Guid Guid)
		{
			EnsureStatusFileHeader();

			var list = new List<StatusItem>();

			using (var csv = Reader.CsvReader(Paths.GetPathItem().StatusFilePath))
			{
				list = csv.GetRecords<StatusItem>().ToList();
			}

			var item = list.First(i => i.Guid.Equals(Guid));
			item.Status = "ready";

			Thread.Sleep(5000);

			using (var csv = WriterUC.Writer(Paths.GetPathItem().StatusFilePath, false))
			{
				csv.WriteRecords(list);
			}
			Console.Write("Calculation is ready and status has been updated!");
		}

		public Guid WriteStatusItem(Guid Guid)
		{
			EnsureStatusFileHeader();

			var path = Paths.GetPathItem().StatusFilePath;

			// Check existing entries and avoid adding duplicate GUIDs
			using (var csvReader = Reader.CsvReader(path))
			{
				var existing = csvReader.GetRecords<StatusItem>();
				if (existing.Any(e => e.Guid.Equals(Guid)))
				{
					// Already present — do not add duplicate
					return Guid;
				}
			}

			using var csv = WriterUC.Writer(path, true);

			csv.WriteRecord(new StatusItem() { Guid = Guid, Status = "not ready" });
			csv.NextRecord();

			return Guid;
		}
	}
}
