using System;
using System.IO;

namespace SearchCSharp
{
	class Program
	{
		static void Main(string[] args)
		{
			// param[0]: string to search
			string searchFor = null;
			if (args.Length >= 1)
			{
				searchFor = args[0];
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

			// should-be-param: Folder to search from (recursively, yet another should-be-param)
			string rootFolder = ".";

			Console.WriteLine($"Searching for strings {searchFor} in .cs files starting from folder {rootFolder}");

			rootFolder = Path.GetFullPath(rootFolder);
			int numOfFilesParsed = 0, numStringsFound = 0;

			var walker = new CSharpStringSearcher(matchString);

			foreach (string filePath in Directory.EnumerateFiles(rootFolder, "*.cs", SearchOption.AllDirectories))
			{
				if (ignoreDesignerFiles && Path.GetFileName(filePath).ToLower().EndsWith(".designer.cs"))
				{
					continue;
				}

				numStringsFound += walker.SearchInFile(filePath);
				numOfFilesParsed++;
			}

			Console.WriteLine($"Processed {numOfFilesParsed} files, found {numStringsFound} matching strings.");
		}
	}
}