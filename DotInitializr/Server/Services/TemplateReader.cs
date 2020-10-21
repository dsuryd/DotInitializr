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

      Dictionary<string, bool> GetComputedTags(TemplateMetadata metadata, Dictionary<string, object> tagValues);

      Dictionary<string, object> GetMetadataTags(TemplateMetadata metadata);

      string GetFilesToExclude(TemplateMetadata metadata, Dictionary<string, bool> conditionalTags);
   }

   public class TemplateReader : ITemplateReader
   {
      private readonly IEnumerable<ITemplateSource> _templateSources;

      private delegate int CountDelegate(params bool[] tags);

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

               // Make sure the tags have keys. Names can be used to substitute keys.
               if (metadata.Tags != null)
               {
                  foreach (var tag in metadata.Tags.Where(x => string.IsNullOrEmpty(x.Key)))
                     tag.Key = tag.Name;
                  metadata.Tags = metadata.Tags.Where(x => !string.IsNullOrEmpty(x.Key));
               }

               if (metadata.ConditionalTags != null)
               {
                  foreach (var tag in metadata.ConditionalTags.Where(x => string.IsNullOrEmpty(x.Key)))
                     tag.Key = tag.Name;
                  metadata.ConditionalTags = metadata.ConditionalTags.Where(x => !string.IsNullOrEmpty(x.Key));
               }

               if (metadata.ComputedTags != null)
                  metadata.ComputedTags = metadata.ComputedTags.Where(x => !string.IsNullOrEmpty(x.Key));
            }
         }

         return metadata;
      }

      public Dictionary<string, bool> GetComputedTags(TemplateMetadata metadata, Dictionary<string, object> tagValues)
      {
         var result = new Dictionary<string, bool>();

         if (metadata.ComputedTags != null)
         {
            var interpreter = new Interpreter();
            foreach (var tag in tagValues)
               interpreter.SetVariable(tag.Key, tag.Value);

            CountDelegate countFunc = Count;
            interpreter.SetFunction("Count", countFunc);

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
                  Trace.TraceError($"Cannot compute `{computedTag.Key}` expression `{computedTag.Expression}`: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
               }
            }
         }

         return result;
      }

      public Dictionary<string, object> GetMetadataTags(TemplateMetadata metadata)
      {
         var result = new Dictionary<string, object>();

         if (metadata?.Tags != null)
            foreach (var tag in metadata.Tags)
               result.Add(tag.Key, tag.DefaultValue);

         if (metadata?.ConditionalTags != null)
            foreach (var tag in metadata.ConditionalTags)
               result.Add(tag.Key, tag.DefaultValue);

         return result;
      }

      public string GetFilesToExclude(TemplateMetadata metadata, Dictionary<string, bool> tags)
      {
         string result = "";

         if (metadata.ConditionalTags != null)
            foreach (var tag in metadata.ConditionalTags.Where(x => !tags.ContainsKey(x.Key) || !tags[x.Key]))
               result = string.Join(',', result, tag.FilesToInclude);

         if (metadata.ComputedTags != null)
            foreach (var tag in metadata.ComputedTags.Where(x => !tags.ContainsKey(x.Key) || !tags[x.Key]))
               result = string.Join(',', result, tag.FilesToInclude);

         return result.Trim(',');
      }

      private int Count(params bool[] tags) => tags.Where(x => x).Count();
   }
}