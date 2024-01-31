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

using DotInitializr.Website.Shared;
using DotNetify;
using DotNetify.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace DotInitializr.Website.Server
{
	public class GeneratorForm : BaseVM
	{
		private readonly IProjectGeneratorV2 _generator;
		private readonly MetadataForm _metadataForm;
		private readonly AppConfigurationV2 _config;
		private readonly ReactiveProperty<string> _templateProp;

		public bool Loading { get => Get<bool>(); set => Set(value); }

		[Ignore]
		public Dictionary<string, string> QueryStrings { get; set; }

		public GeneratorForm(IProjectGeneratorV2 generator, AppConfigurationV2 config, MetadataForm metadataForm)
		{
			_generator = generator;
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
			return template != null ? _generator.BuildProjectMetadata(formData, metadata, template) : null;
		}
	}
}