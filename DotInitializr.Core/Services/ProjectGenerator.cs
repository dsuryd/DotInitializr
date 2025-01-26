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
      ProjectMetadata BuildProjectMetadata(Dictionary<string, string> formData, TemplateMetadata metadata, AppConfiguration.Template template);

      byte[] Generate(ProjectMetadata metadata);
   }

   public class ProjectGenerator : IProjectGenerator
   {
      private readonly IEnumerable<ITemplateSource> _templateSources;
      private readonly IEnumerable<ITemplateRenderer> _renderers;
      private readonly ITemplateMetadataReader _templateReader;

      public ProjectGenerator(IEnumerable<ITemplateSource> templateSources, IEnumerable<ITemplateRenderer> renderers, ITemplateMetadataReader templateReader)
      {
         _templateSources = templateSources;
         _renderers = renderers;
         _templateReader = templateReader;
      }

      public ProjectMetadata BuildProjectMetadata(Dictionary<string, string> formData, TemplateMetadata metadata, AppConfiguration.Template template)
      {
         var tags = _templateReader.GetTags(metadata);
         var conditionalTags = _templateReader.GetConditionalTags(metadata);

         foreach (var key in formData.Keys.Where(x => tags.ContainsKey(x)))
         {
            string value = formData[key]?.ToString();
            try
            {
               if (tags[key] is int)
                  tags[key] = int.TryParse(value, out int intValue) ? intValue : tags[key];
               else if (tags[key] is float)
                  tags[key] = float.TryParse(value, out float floatValue) ? floatValue : tags[key];
               else
                  tags[key] = value;
            }
            catch (Exception)
            {
               tags[key] = value;
            }
         }

         foreach (var key in formData.Keys.Where(x => conditionalTags.ContainsKey(x)))
         {
            if (bool.TryParse(formData[key]?.ToString(), out bool value))
               conditionalTags[key] = value;
         }

         var nonComputedTags = tags.ToDictionary(x => x.Key, x => x.Value)
            .Union(conditionalTags.ToDictionary(x => x.Key, x => (object) x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

         var computedTags = _templateReader.GetComputedTags(metadata, nonComputedTags);
         var boolComputedTags = computedTags.Where(x => x.Value is bool).ToDictionary(x => x.Key, x => (bool) x.Value);
         var booleanTags = conditionalTags.Union(boolComputedTags).ToDictionary(x => x.Key, x => x.Value);

         var allTags = nonComputedTags
            .Union(computedTags.ToDictionary(x => x.Key, x => x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

         string projectName = TemplateMetadataReader.DEFAULT_PROJECT_NAME;
         if (nonComputedTags.ContainsKey(TemplateMetadataReader.PROJECT_NAME_KEY))
            projectName = nonComputedTags[TemplateMetadataReader.PROJECT_NAME_KEY].ToString();
         else
         {
            var projectNameTag = metadata.Tags.FirstOrDefault(x => x.Name == TemplateMetadataReader.PROJECT_NAME);
            if (projectNameTag != null)
               projectName = nonComputedTags[projectNameTag.Key].ToString();
         }

         return new ProjectMetadata
         {
            ProjectName = projectName,
            TemplateType = metadata.TemplateType,
            TemplateSourceType = template.SourceType,
            TemplateSourceUrl = template.SourceUrl,
            TemplateSourceDirectory = template.SourceDirectory,
            TemplateSourceBranch = template.SourceBranch,
            FilesToExclude = _templateReader.GetFilesToExclude(metadata, booleanTags),
            Tags = allTags,
            TagRegexes = _templateReader.GetTagRegexes(metadata)
         };
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
            .GetFiles(metadata.TemplateSourceUrl, metadata.TemplateSourceDirectory, metadata.TemplateSourceBranch, 1)
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
         var directories = files.Where(x => x.EndsWith(wildcard))?.Select(x => x.Replace(wildcard, $"{Path.DirectorySeparatorChar}"));
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
               using var zippedEntry = new MemoryStream(file is TemplateFileBinary ? (file as TemplateFileBinary).ContentBytes : Encoding.UTF8.GetBytes(file.Content));
               entry.ExternalAttributes = 27262976; // RW_(Owner)/R__(Group)/___(Other)
               zippedEntry.CopyTo(entryStream);
            }
         }

         return memoryStream.ToArray();
      }
   }
}