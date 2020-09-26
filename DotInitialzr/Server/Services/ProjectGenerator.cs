using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DotInitialzr.Shared;

namespace DotInitialzr.Server
{
   public interface IProjectGenerator
   {
      byte[] Generate(TemplateMetadata metadata);
   }

   public class ProjectGenerator : IProjectGenerator
   {
      private readonly IEnumerable<ITemplateSource> _templateSources;

      public ProjectGenerator(IEnumerable<ITemplateSource> templateSources)
      {
         _templateSources = templateSources;
      }

      public byte[] Generate(TemplateMetadata metadata)
      {
         var templateSource = _templateSources.FirstOrDefault(x => x.SourceType == metadata.TemplateSourceType);
         if (templateSource == null)
            throw new Exception($"Template source '{metadata.TemplateSourceType}' is not found");

         var files = templateSource.GetFiles(metadata.TemplateSourceUrl, metadata.TemplateSourceDirectory);

         return Zip(files);
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