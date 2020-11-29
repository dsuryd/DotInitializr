/*
Copyright 2020 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace DotInitializr
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

         if (sourceDirectory != null)
            sourceDirectory = sourceDirectory.Replace('\\', Path.DirectorySeparatorChar);

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
            var error = $"Failed to get file `{fileName}` from `{sourceUrl}`";
            Console.WriteLine(error + Environment.NewLine + ex.ToString());
            throw new TemplateException(error);
         }

         Utils.DeleteDirectory(tempPath);
         return result;
      }

      public IEnumerable<TemplateFile> GetFiles(string sourceUrl, string sourceDirectory = null)
      {
         List<TemplateFile> result = new List<TemplateFile>();
         string tempPath = Path.Combine(Path.GetTempPath(), nameof(DotInitializr), Guid.NewGuid().ToString());

         if (sourceDirectory != null)
            sourceDirectory = sourceDirectory.Replace('\\', Path.DirectorySeparatorChar);

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
            var error = $"Failed to get files from `{sourceUrl}`";
            Console.WriteLine(error + Environment.NewLine + ex.ToString());
            throw new TemplateException(error);
         }

         Utils.DeleteDirectory(tempPath);
         return result;
      }
   }
}