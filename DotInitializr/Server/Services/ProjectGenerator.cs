using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DotInitializr.Shared;

namespace DotInitializr.Server
{
   public interface IProjectGenerator
   {
      byte[] Generate(ProjectMetadata metadata);
   }

   public class ProjectGenerator : IProjectGenerator
   {
      private readonly IEnumerable<ITemplateSource> _templateSources;
      private readonly ITemplateRenderer _mustacheRenderer;

      public ProjectGenerator(IEnumerable<ITemplateSource> templateSources, ITemplateRenderer mustacheRenderer)
      {
         _templateSources = templateSources;
         _mustacheRenderer = mustacheRenderer;
      }

      public byte[] Generate(ProjectMetadata metadata)
      {
         var templateSource = _templateSources.FirstOrDefault(x => string.Equals(x.SourceType, metadata.TemplateSourceType, StringComparison.InvariantCultureIgnoreCase));
         if (templateSource == null)
            throw new Exception($"Template source '{metadata.TemplateSourceType}' is not found");

         var filesToExclude = metadata.FilesToExclude?
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Split(",");

         var files = templateSource
            .GetFiles(metadata.TemplateSourceUrl, metadata.TemplateSourceDirectory)
            .Where(x => filesToExclude == null || !MatchFileName(x.Name, filesToExclude))
            .ToList();

         return Zip(_mustacheRenderer.Render(files, metadata.Tags));
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