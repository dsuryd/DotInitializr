using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotInitializr.Shared;
using DotNetify;
using DotNetify.Elements;

namespace DotInitializr.Server
{
   public class GeneratorForm : BaseVM
   {
      private readonly ITemplateReader _templateReader;
      private readonly MetadataForm _metadataForm;
      private readonly AppConfiguration _config;
      private readonly ReactiveProperty<string> _templateProp;
      private Dictionary<string, object> _metadataTags;

      public bool Loading { get => Get<bool>(); set => Set(value); }

      public GeneratorForm(ITemplateReader templateReader, AppConfiguration config, MetadataForm metadataForm)
      {
         _templateReader = templateReader;
         _config = config;
         _metadataForm = metadataForm;
         _metadataForm.MetadataLoadedEvent.Subscribe(_ => Loading = false);

         _templateProp = AddProperty("Template", "none")
            .WithAttribute(new DropdownListAttribute
            {
               Label = "Template",
               Placeholder = "Select one...",
               Options = config.Templates.Select(x => KeyValuePair.Create(x.Key, x.Name)).Prepend(KeyValuePair.Create("none", "")).ToArray()
            })
            .WithServerValidation(x => true, string.Empty)  // Add this so that input field change is dispatched to the server VM.
            .SubscribedBy(_metadataForm.TemplateChangedEvent, key =>
            {
               Loading = true;
               PushUpdates();

               _metadataTags = null;
               return _config.Templates.FirstOrDefault(x => x.Key == key);
            })
            .SubscribedBy(AddProperty<string>("Description"), key => _config.Templates.FirstOrDefault(x => x.Key == key)?.Description);

         AddInternalProperty<Dictionary<string, string>>("Generate")
            .WithAttribute(new { Label = "Generate" })
            .Merge(AddInternalProperty<Dictionary<string, string>>(nameof(IGeneratorFormState.ClearProjectMetadata)))
            .Select(formData => formData != null ? BuildProjectMetadata(formData, _metadataForm.ActiveMetadata) : null)
            .Subscribe(AddProperty<ProjectMetadata>(nameof(IGeneratorFormState.ProjectMetadata)));
      }

      public override BaseVM GetSubVM(string vmTypeName)
      {
         if (vmTypeName == nameof(MetadataForm))
            return _metadataForm;

         return base.GetSubVM(vmTypeName);
      }

      private ProjectMetadata BuildProjectMetadata(Dictionary<string, string> formData, TemplateMetadata metadata)
      {
         var template = _config.Templates.FirstOrDefault(x => x.Key == _templateProp.Value.ToString());
         if (template == null)
            return null;

         _metadataTags ??= _templateReader.GetMetadataTags(metadata);
         foreach (var key in formData.Keys.Where(x => _metadataTags.ContainsKey(x)))
         {
            if (_metadataTags[key] is bool)
            {
               bool.TryParse(formData[key]?.ToString(), out bool value);
               _metadataTags[key] = value;
            }
            else
               _metadataTags[key] = formData[key]?.ToString();
         }

         var conditionalTags = _metadataTags.Where(x => x.Value is bool).ToDictionary(x => x.Key, x => (bool) x.Value);
         var tags = _metadataTags
            .Union(_templateReader.GetComputedTags(metadata, conditionalTags).ToDictionary(x => x.Key, x => (object) x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

         return new ProjectMetadata
         {
            ProjectName = _metadataTags[MetadataForm.ProjectNameKey].ToString(),
            TemplateSourceType = template.SourceType,
            TemplateSourceUrl = template.SourceUrl,
            TemplateSourceDirectory = template.SourceDirectory,
            FilesToExclude = _templateReader.GetFilesToExclude(metadata, conditionalTags),
            Tags = tags,
         };
      }
   }
}