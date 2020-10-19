using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace DotInitializr.Server
{
   public class GitTemplateSource : ITemplateSource
   {
      private static readonly List<string> _ignoreFiles = new List<string>
      {
         Path.DirectorySeparatorChar + ".git",
         Path.AltDirectorySeparatorChar + ".git"
      };

      public string SourceType => "git";

      public TemplateFile GetFile(string fileName, string sourceUrl, string sourceDirectory = null)
      {
         TemplateFile result = null;
         string tempPath = Path.Combine(Path.GetTempPath(), nameof(DotInitializr), Guid.NewGuid().ToString());

         try
         {
            if (!string.IsNullOrEmpty(Repository.Clone(sourceUrl, tempPath, new CloneOptions { Checkout = false })))
            {
               var filePath = string.IsNullOrEmpty(sourceDirectory) ? fileName : Path.Combine(sourceDirectory, fileName);

               using var repo = new Repository(tempPath);
               repo.CheckoutPaths(repo.Head.FriendlyName, new string[] { filePath }, new CheckoutOptions());

               result = new TemplateFile
               {
                  Name = filePath,
                  Content = File.ReadAllText(Path.Combine(tempPath, filePath))
               };
            }
         }
         catch (Exception ex)
         {
            Trace.TraceError($"Failed to get file `{fileName}` from `{sourceUrl}`: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
         }

         Utils.DeleteDirectory(tempPath);
         return result;
      }

      public IEnumerable<TemplateFile> GetFiles(string sourceUrl, string sourceDirectory = null)
      {
         List<TemplateFile> result = new List<TemplateFile>();
         string tempPath = Path.Combine(Path.GetTempPath(), nameof(DotInitializr), Guid.NewGuid().ToString());

         try
         {
            string fullTempPath = Path.Combine(Path.GetFullPath(tempPath), sourceDirectory);

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
                        .Replace(fullTempPath, string.Empty)
                        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                     Content = File.ReadAllText(fileName)
                  });
               }
            }
         }
         catch (Exception ex)
         {
            Trace.TraceError($"Failed to get files from `{sourceUrl}`: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
         }

         Utils.DeleteDirectory(tempPath);
         return result;
      }
   }
}