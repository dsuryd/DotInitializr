using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using DotInitialzr.Shared;
using DotNetify;
using DotNetify.Elements;

namespace DotInitialzr.Server
{
   public class MetadataForm : BaseVM
   {
      private static readonly string DefaultProjectName = "Starter";
      private readonly IEnumerable<ITemplateSource> _templateSources;
      private readonly ReactiveProperty<TemplateMetadata> Metadata;

      [Ignore]
      public ReactiveProperty<AppConfiguration.Template> TemplateChangedEvent { get; } = new ReactiveProperty<AppConfiguration.Template>();

      public MetadataForm(IEnumerable<ITemplateSource> templateSources)
      {
         _templateSources = templateSources;

         AddProperty("ProjectName", DefaultProjectName)
            .WithAttribute(new TextFieldAttribute
            {
               Label = "Project Name:",
               Placeholder = "Enter your project name",
               MaxLength = 30
            })
            .WithPatternValidation(@"^[\w\-. ]+$", "Must be a valid filename")
            .WithRequiredValidation();

         Metadata = AddInternalProperty<TemplateMetadata>("Metadata")
            .SubscribeTo(TemplateChangedEvent.Select(x => GetMetadata(x)))
            .SubscribedBy(AddProperty<IEnumerable<string>>(nameof(IMetadataFormState.TextFields)), metadata => BuildTextFieldProperties(metadata))
            .SubscribedBy(AddProperty<IEnumerable<string>>(nameof(IMetadataFormState.Checkboxes)), metadata => BuildCheckboxProperties(metadata));
      }

      public Dictionary<string, object> GetDefaultMetadata()
      {
         var result = new Dictionary<string, object>()
         {
            { "ProjectName", DefaultProjectName }
         };
         var metadata = Metadata.Value as TemplateMetadata;

         if (metadata?.TextTags != null)
            foreach (var tag in metadata.TextTags)
               result.Add(tag.Key, tag.DefaultValue);

         if (metadata?.ConditionalTags != null)
            foreach (var tag in metadata.ConditionalTags)
               result.Add(tag.Key, tag.DefaultValue);

         return result;
      }

      private IEnumerable<string> BuildTextFieldProperties(TemplateMetadata metadata)
      {
         var textFieldIds = new List<string>();

         if (metadata.TextTags != null)
         {
            foreach (var tag in metadata.TextTags)
            {
               string name = tag.Key;
               RemoveExistingProperty(name);
               textFieldIds.Add(name);

               var prop = AddProperty(name, tag.DefaultValue)
                  .WithAttribute(new TextFieldAttribute
                  {
                     Label = tag.Name + ":"
                  });

               if (!string.IsNullOrEmpty(tag.ValidationRegex))
                  prop.WithPatternValidation(tag.ValidationRegex, tag.ValidationError);

               RegisterPropertyAttributes(name);
            }
         }

         return textFieldIds;
      }

      private IEnumerable<string> BuildCheckboxProperties(TemplateMetadata metadata)
      {
         var checkboxIds = new List<string>();

         if (metadata.ConditionalTags != null)
         {
            foreach (var tag in metadata.ConditionalTags)
            {
               string name = tag.Key;
               RemoveExistingProperty(name);
               checkboxIds.Add(name);

               AddProperty(name, tag.DefaultValue)
                  .WithAttribute(new CheckboxAttribute
                  {
                     Label = tag.Name
                  });

               RegisterPropertyAttributes(name);
            }
         }

         return checkboxIds;
      }

      private TemplateMetadata GetMetadata(AppConfiguration.Template template)
      {
         TemplateMetadata metadata = new TemplateMetadata();

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

      private void RegisterPropertyAttributes(string propName)
      {
         // Properties that are added after view model instantiation must be manually set so that
         // their values will be included in the updates to the client.
         foreach (var metaProp in RuntimeProperties.Where(x => x.Name.StartsWith(propName + "__")))
            Set(metaProp.Value, metaProp.Name);
      }

      private void RemoveExistingProperty(string propName)
      {
         RuntimeProperties.Where(x => x.Name == propName || x.Name.Contains(propName + "__"))
            .ToList()
            .ForEach(x => RuntimeProperties.Remove(x));

         _propertyValues.TryRemove(propName, out object value);
      }
   }
}