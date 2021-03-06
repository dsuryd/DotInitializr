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
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DotInitializr
{
   /// <summary>
   /// Generates a zipped project from a metadata.
   /// </summary>
   public interface IProjectGenerator
   {
      byte[] Generate(ProjectMetadata metadata);
   }

   public class ProjectGenerator : IProjectGenerator
   {
      private readonly IEnumerable<ITemplateSource> _templateSources;
      private readonly IEnumerable<ITemplateRenderer> _renderers;

      public ProjectGenerator(IEnumerable<ITemplateSource> templateSources, IEnumerable<ITemplateRenderer> renderers)
      {
         _templateSources = templateSources;
         _renderers = renderers;
      }

      public byte[] Generate(ProjectMetadata metadata)
      {
         var renderer = _renderers.FirstOrDefault(x => string.Compare(x.TemplateType, metadata.TemplateType ?? MustacheRenderer.TEMPLATE_TYPE, true) == 0);

         var templateSource = _templateSources.FirstOrDefault(x => string.Equals(x.SourceType, metadata.TemplateSourceType, StringComparison.InvariantCultureIgnoreCase));
         if (templateSource == null)
            throw new Exception($"Template source '{metadata.TemplateSourceType}' is not found");

         var filesToExclude = string.Join(",", metadata.FilesToExclude, TemplateMetadata.FILE_NAME)
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Trim(',')
            .Split(",");

         var files = templateSource
            .GetFiles(metadata.TemplateSourceUrl, metadata.TemplateSourceDirectory)
            .Where(x => filesToExclude == null || !MatchFileName(x.Name, filesToExclude))
            .ToList();

         return Zip(renderer.Render(files, metadata.Tags, metadata.TagRegexes));
      }

      private bool MatchFileName(string name, string[] files)
      {
         name = name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

         if (files.Contains(name))
            return true;

         var wildcard = $"{Path.DirectorySeparatorChar}**";
         var directories = files.Where(x => x.EndsWith(wildcard))?.Select(x => x.Replace(wildcard, string.Empty));
         return directories?.Any(x => name.StartsWith(x)) == true;
      }

      private byte[] Zip(IEnumerable<TemplateFile> files)
      {
         using var memoryStream = new MemoryStream();
         using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
         {
            foreach (var file in files)
            {
               var entry = archive.CreateEntry(file.Name, CompressionLevel.Optimal);
               using var entryStream = entry.Open();
               using var zippedEntry = new MemoryStream(Encoding.UTF8.GetBytes(file.Content));
               entry.ExternalAttributes = 27262976; // RW_(Owner)/R__(Group)/___(Other)
               zippedEntry.CopyTo(entryStream);
            }
         }

         return memoryStream.ToArray();
      }
   }
}