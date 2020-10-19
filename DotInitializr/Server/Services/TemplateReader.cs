using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using DynamicExpresso;

namespace DotInitializr.Server
{
   public interface ITemplateReader
   {
      TemplateMetadata GetMetadata(AppConfiguration.Template template);

      Dictionary<string, bool> GetComputedTags(TemplateMetadata metadata, Dictionary<string, bool> conditionalTags);

      Dictionary<string, object> GetMetadataTags(TemplateMetadata metadata);

      string GetFilesToExclude(TemplateMetadata metadata, Dictionary<string, bool> conditionalTags);
   }

   public class TemplateReader : ITemplateReader
   {
      private readonly IEnumerable<ITemplateSource> _templateSources;

      public TemplateReader(IEnumerable<ITemplateSource> templateSources)
      {
         _templateSources = templateSources;
      }

      public TemplateMetadata GetMetadata(AppConfiguration.Template template)
      {
         TemplateMetadata metadata = null;

         var templateSource = _templateSources.FirstOrDefault(x => string.Equals(x.SourceType, template?.SourceType, StringComparison.InvariantCultureIgnoreCase));
         if (templateSource != null)
         {
            var metadataFile = templateSource.GetFile(TemplateMetadata.FILE_NAME, template.SourceUrl, template.SourceDirectory);
            if (metadataFile != null && !string.IsNullOrEmpty(metadataFile.Content))
            {
               try
               {
                  metadata = JsonSerializer.Deserialize<TemplateMetadata>(metadataFile.Content);
               }
               catch (Exception ex)
               {
                  Trace.TraceError($"`{TemplateMetadata.FILE_NAME}` in `{template.SourceUrl}` must be in JSON: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
               }
            }
         }

         return metadata;
      }

      public Dictionary<string, bool> GetComputedTags(TemplateMetadata metadata, Dictionary<string, bool> conditionalTags)
      {
         var result = new Dictionary<string, bool>();

         if (metadata.ComputedTags != null)
         {
            var interpreter = new Interpreter();
            foreach (var tag in conditionalTags)
               interpreter.SetVariable(tag.Key, tag.Value);

            foreach (var computedTag in metadata.ComputedTags)
            {
               try
               {
                  object value = interpreter.Eval(computedTag.Expression);
                  if (value is bool && (bool) value == true)
                     result.Add(computedTag.Key, true);
               }
               catch (Exception ex)
               {
                  Trace.TraceError($"Cannot compute expression `{computedTag.Expression}` for `{computedTag.Key}`: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
               }
            }
         }

         return result;
      }

      public Dictionary<string, object> GetMetadataTags(TemplateMetadata metadata)
      {
         var result = new Dictionary<string, object>();

         if (metadata?.TextTags != null)
            foreach (var tag in metadata.TextTags)
               result.Add(tag.Key, tag.DefaultValue);

         if (metadata?.ConditionalTags != null)
            foreach (var tag in metadata.ConditionalTags)
               result.Add(tag.Key, tag.DefaultValue);

         return result;
      }

      public string GetFilesToExclude(TemplateMetadata metadata, Dictionary<string, bool> conditionalTags)
      {
         string result = "";

         foreach (var tag in metadata.ConditionalTags.Where(x => !conditionalTags.ContainsKey(x.Key) || !conditionalTags[x.Key]))
            result = string.Join(',', result, tag.FilesToInclude);

         return result.Trim(',');
      }
   }
}