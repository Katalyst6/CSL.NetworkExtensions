using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace CodePackager
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceDirectory = new DirectoryInfo(@"D:\Developpement\Git\CSL.NetworkExtensions");
            var targetFile = "Sources.cs";

            var headers = new List<string>();
            var content = new List<string>();

            foreach (var codeFile in sourceDirectory
                .GetFiles("*.cs", SearchOption.AllDirectories)
                .Where(fi => fi.Name != "AssemblyInfo.cs")
                .Where(fi => fi.Name != "Program.cs")
                .OrderBy(fi => fi.FullName))
            {
                var fileContent = File.ReadAllLines(codeFile.FullName);

                var lineId = 0;

                for (; lineId < fileContent.Length; lineId++)
                {
                    var line = fileContent[lineId];

                    if (line.ToUpper().StartsWith("USING ") ||
                        line.ToUpper().StartsWith("#IF") ||
                        line.ToUpper().StartsWith("#ELSE") ||
                        line.ToUpper().StartsWith("#ENDIF"))
                    {
                        headers.Add(line);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    break;
                }

                if (content.Count != 0)
                {
                    content.Add("");
                    content.Add("");
                }

                for (; lineId < fileContent.Length; lineId++)
                {
                    content.Add(fileContent[lineId]);
                }
            }

            headers = TrimHeaders(headers);
            var result = new StringBuilder();

            foreach (var lines in headers)
            {
                result.AppendLine(lines);
            }

            result.AppendLine();
            result.AppendLine();

            foreach (var lines in content)
            {
                result.AppendLine(lines);
            }

            System.Diagnostics.Debugger.Break();
        }

        private static List<string> TrimHeaders(List<string> headers)
        {
            var list = new List<string>();

            for (int i = 0; i < headers.Count(); i++)
            {
                var line = headers.ElementAt(i);

                if (line.Contains("NetworkExtensions"))
                {
                    continue;
                }

                if (line.ToUpper().StartsWith("#IF"))
                {
                    //list.Add(line);

                    while (i + 1 < headers.Count())
                    {
                        i++;
                        line = headers.ElementAt(i);

                        //list.Add(line);

                        if (line.ToUpper().StartsWith("#ENDIF"))
                        {
                            break;
                        }
                    }

                    continue;
                }

                if (!list.Contains(line))
                {
                    list.Add(line);
                }
            }

            return list.OrderBy(x => x).ToList();
        }
    }
}
