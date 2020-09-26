using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace DotInitialzr.Server
{
   public class GitTemplateSource : ITemplateSource
   {
      private static readonly List<string> _ignoreFiles = new List<string>
      {
         Path.DirectorySeparatorChar + ".git",
         Path.AltDirectorySeparatorChar + ".git"
      };

      public string SourceType => "git";

      public IEnumerable<TemplateFile> GetFiles(string sourceUrl, string sourceDirectory = null)
      {
         List<TemplateFile> result = new List<TemplateFile>();

         try
         {
            string tempPath = Path.Combine(Path.GetTempPath(), nameof(DotInitialzr), Guid.NewGuid().ToString());

            if (!string.IsNullOrEmpty(Repository.Clone(sourceUrl, tempPath)))
            {
               string filePath = string.IsNullOrEmpty(sourceDirectory) ? tempPath : Path.Combine(tempPath, sourceDirectory);
               foreach (var fileName in Directory.EnumerateFiles(filePath, "*", SearchOption.AllDirectories))
               {
                  if (_ignoreFiles.Any(x => fileName.Contains(x)))
                     continue;

                  result.Add(new TemplateFile
                  {
                     Name = fileName
                        .Replace(Path.GetFullPath(tempPath), string.Empty)
                        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                     Content = File.ReadAllText(fileName)
                  });
               }
            }

            Utils.DeleteDirectory(tempPath);
         }
         catch (Exception ex)
         {
            Trace.TraceError($"Failed to get files from `{sourceUrl}`: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
         }

         return result;
      }
   }
}