using System.Collections.Generic;
using System.Linq;
using DotInitialzr.Shared;
using DotNetify;
using DotNetify.Elements;

namespace DotInitialzr.Server
{
   public class Generator : BaseVM
   {
      private static readonly string DefaultProjectName = "Starter";
      private readonly InitialzrConfiguration _config;

      private class FormData
      {
         public string Template { get; set; }
         public string ProjectName { get; set; }
      }

      public Generator(InitialzrConfiguration config)
      {
         _config = config;

         AddProperty<string>(nameof(FormData.Template))
            .WithAttribute(new DropdownListAttribute
            {
               Label = "Template:",
               Placeholder = "Select one...",
               Options = config.Templates.Select(x => KeyValuePair.Create(x.Key, x.Name)).Prepend(KeyValuePair.Create("none", "")).ToArray()
            })
            .WithRequiredValidation();

         AddProperty<string>(nameof(FormData.ProjectName), DefaultProjectName)
            .WithAttribute(new TextFieldAttribute
            {
               Label = "Project Name:",
               Placeholder = "Enter your project name",
               MaxLength = 30
            })
            .WithPatternValidation(@"^[\w\-. ]+$", "Must be a valid filename")
            .WithRequiredValidation();

         AddProperty<FormData>("Generate")
         .WithAttribute(new { Label = "Generate" })
         .SubscribedBy(
            AddProperty<ProjectMetadata>(nameof(IGeneratorState.ProjectMetadata)), formData => BuildProjectMetadata(formData));
      }

      private ProjectMetadata BuildProjectMetadata(FormData formData)
      {
         var template = _config.Templates.FirstOrDefault(x => x.Key == formData.Template);

         var tags = new Dictionary<string, object>
         {
            { "projectName", formData.ProjectName }
         };

         return new ProjectMetadata
         {
            ProjectName = formData.ProjectName,
            TemplateSourceType = template.SourceType,
            TemplateSourceUrl = template.SourceUrl,
            TemplateSourceDirectory = template.SourceDirectory,
            Tags = tags
         };
      }
   }
}