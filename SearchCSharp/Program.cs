using System;
using System.Collections.Generic;
using System.IO;

namespace SearchCSharp
{
	class Program
	{
		static int Main(string[] args)
		{
			// Folder to search from
			string rootFolder = ".";

			// param[0]: string to search
			string searchFor = null;

			if (args.Length >= 1)
			{
				searchFor = args[0];
			}

			// param[1]: folder to search in
			if (args.Length >= 2)
			{
				rootFolder = args[1];
			}

			if (args.Length >= 3)
			{
				PrintUsage();
				return 1;
			}

			// should-be-param: Ignore *.designer.cs files when searching
			bool ignoreDesignerFiles = true;

			// should-be-param: Ignore case when searching
			bool ignoreCase = true;

			Func<string, bool> matchString = null;
			if (searchFor != null)
			{
				if (!ignoreCase)
				{
					matchString =
						str => str.Contains(searchFor);
				}
				else
				{
					matchString =
						str => str.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) >= 0;
				}
			}


			Console.WriteLine($"Searching for strings {searchFor} in .cs files starting from folder {rootFolder}");

			rootFolder = Path.GetFullPath(rootFolder);
			int numOfFilesParsed = 0, numStringsFound = 0;

			var walker = new CSharpStringSearcher(matchString);

			IEnumerable<string> allFiles;

			try
			{
				allFiles = Directory.EnumerateFiles(rootFolder, "*.cs", SearchOption.AllDirectories);
			}
			catch (DirectoryNotFoundException ex)
			{
				Console.WriteLine($"Directory not found: {ex.Message}");
				return 2;
			}

			foreach (string filePath in allFiles)
			{
				if (ignoreDesignerFiles &&
					Path.GetFileName(filePath).EndsWith(".designer.cs", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				numStringsFound += walker.SearchInFile(filePath);
				numOfFilesParsed++;
			}

			Console.WriteLine($"Processed {numOfFilesParsed} files, found {numStringsFound} matching strings.");

			return 0;
		}

		static void PrintUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("SearchCSharp string-to-search [folder-to-search]");
		}
	}
}