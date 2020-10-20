using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotInitializr.Shared;
using DotNetify;
using DotNetify.Elements;

namespace DotInitializr.Server
{
   public class MetadataForm : BaseVM
   {
      public const string ProjectNameKey = "projectName";
      public const string DefaultProjectName = "Starter";

      private readonly ITemplateReader _templateReader;

      [Ignore]
      public ReactiveProperty<TemplateMetadata> MetadataLoadedEvent { get; } = new ReactiveProperty<TemplateMetadata>();

      [Ignore]
      public ReactiveProperty<AppConfiguration.Template> TemplateChangedEvent { get; } = new ReactiveProperty<AppConfiguration.Template>();

      [Ignore]
      public TemplateMetadata ActiveMetadata => MetadataLoadedEvent.Value as TemplateMetadata;

      public MetadataForm(ITemplateReader templateReader)
      {
         _templateReader = templateReader;

         MetadataLoadedEvent
            .SubscribeTo(TemplateChangedEvent.Select(x => GetMetadata(x)))
            .SubscribedBy(AddProperty<IEnumerable<string>>(nameof(IMetadataFormState.TextFields)), metadata => BuildTextFieldProperties(metadata))
            .SubscribedBy(AddProperty<IEnumerable<string>>(nameof(IMetadataFormState.Checkboxes)), metadata => BuildCheckboxProperties(metadata));
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
                     Label = tag.Name,
                     Placeholder = tag.Description
                  })
                  .WithRequiredValidation();

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
                  .WithAttribute(new
                  {
                     Label = tag.Name,
                     tag.Description
                  });

               RegisterPropertyAttributes(name);
            }
         }

         return checkboxIds;
      }

      private TemplateMetadata GetMetadata(AppConfiguration.Template template)
      {
         var metadata = _templateReader.GetMetadata(template) ?? new TemplateMetadata();

         if (template != null)
         {
            var textTags = metadata.TextTags?.ToList() ?? new List<TextTemplateTag>();

            textTags.Insert(0, new TextTemplateTag
            {
               Key = ProjectNameKey,
               Name = "Project Name",
               DefaultValue = DefaultProjectName,
               ValidationRegex = @"^[\w\-. ]+$",
               ValidationError = "Must be a valid filename",
            });

            metadata.TextTags = textTags;
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