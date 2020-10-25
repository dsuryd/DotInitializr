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
      private Dictionary<string, string> _tags;
      private Dictionary<string, bool> _conditionalTags;

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

               _tags = null;
               _conditionalTags = null;
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

         _tags ??= _templateReader.GetTags(metadata);
         _conditionalTags ??= _templateReader.GetConditionalTags(metadata);

         foreach (var key in formData.Keys.Where(x => _tags.ContainsKey(x)))
            _tags[key] = formData[key]?.ToString();

         foreach (var key in formData.Keys.Where(x => _conditionalTags.ContainsKey(x)))
         {
            if (bool.TryParse(formData[key]?.ToString(), out bool value))
               _conditionalTags[key] = value;
         }

         var nonComputedTags = _tags.ToDictionary(x => x.Key, x => (object) x.Value)
            .Union(_conditionalTags.ToDictionary(x => x.Key, x => (object) x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

         var computedTags = _templateReader.GetComputedTags(metadata, nonComputedTags);
         var booleanTags = _conditionalTags.Union(computedTags).ToDictionary(x => x.Key, x => x.Value);

         var allTags = nonComputedTags
            .Union(computedTags.ToDictionary(x => x.Key, x => (object) x.Value))
            .ToDictionary(x => x.Key, x => x.Value);

         return new ProjectMetadata
         {
            ProjectName = nonComputedTags[MetadataForm.ProjectNameKey].ToString(),
            TemplateSourceType = template.SourceType,
            TemplateSourceUrl = template.SourceUrl,
            TemplateSourceDirectory = template.SourceDirectory,
            FilesToExclude = _templateReader.GetFilesToExclude(metadata, booleanTags),
            Tags = allTags,
         };
      }
   }
}