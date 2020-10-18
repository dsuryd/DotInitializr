using System.Collections.Generic;
using System.Linq;
using DotInitialzr.Shared;
using DotNetify;
using DotNetify.Elements;

namespace DotInitialzr.Server
{
   public class GeneratorForm : BaseVM
   {
      private readonly MetadataForm _metadataForm;
      private readonly AppConfiguration _config;

      public GeneratorForm(AppConfiguration config, MetadataForm metadataForm)
      {
         _config = config;
         _metadataForm = metadataForm;

         AddProperty<string>(nameof(IGeneratorFormState.Template), "none")
            .WithAttribute(new DropdownListAttribute
            {
               Label = "Template:",
               Placeholder = "Select one...",
               Options = config.Templates.Select(x => KeyValuePair.Create(x.Key, x.Name)).Prepend(KeyValuePair.Create("none", "")).ToArray()
            })
            .WithRequiredValidation()
            .WithServerValidation(x => true, string.Empty)  // Add this so that input field change is dispatched to the server VM.
            .SubscribedBy(
               AddProperty<bool>(nameof(IGeneratorFormState.ShowMetadataForm)), templateKey => !string.IsNullOrEmpty(templateKey))
            .SubscribedBy(_metadataForm.TemplateChangedEvent, templateKey => _config.Templates.FirstOrDefault(x => x.Key == templateKey));

         AddProperty<Dictionary<string, string>>(nameof(IGeneratorFormState.Generate))
            .WithAttribute(new { Label = "Generate" })
            .SubscribedBy(
               AddProperty<ProjectMetadata>(nameof(IGeneratorFormState.ProjectMetadata)), formData => BuildProjectMetadata(formData));
      }

      public override BaseVM GetSubVM(string vmTypeName)
      {
         if (vmTypeName == nameof(MetadataForm))
            return _metadataForm;

         return base.GetSubVM(vmTypeName);
      }

      private ProjectMetadata BuildProjectMetadata(Dictionary<string, string> formData)
      {
         var template = _config.Templates.FirstOrDefault(x => x.Key == formData[nameof(IGeneratorFormState.Template)]);

         var tags = new Dictionary<string, object>();

         foreach (var metadata in _metadataForm.GetDefaultMetadata())
            tags.Add(metadata.Key, formData.ContainsKey(metadata.Key) ? formData[metadata.Key] : metadata.Value);

         return new ProjectMetadata
         {
            ProjectName = tags[nameof(IMetadataFormState.ProjectName)].ToString(),
            TemplateSourceType = template.SourceType,
            TemplateSourceUrl = template.SourceUrl,
            TemplateSourceDirectory = template.SourceDirectory,
            Tags = tags
         };
      }
   }
}