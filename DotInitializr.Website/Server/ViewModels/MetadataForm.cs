﻿/*
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

using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DotInitializr.Website.Shared;
using DotNetify;
using DotNetify.Elements;

namespace DotInitializr.Website.Server
{
    public class MetadataForm : BaseVM
    {
        private readonly ITemplateMetadataReader _templateReader;

        [Ignore]
        public ReactiveProperty<TemplateMetadata> MetadataLoadedEvent { get; } = new ReactiveProperty<TemplateMetadata>();

        [Ignore]
        public ReactiveProperty<AppConfiguration.Template> TemplateChangedEvent { get; } = new ReactiveProperty<AppConfiguration.Template>();

        [Ignore]
        public TemplateMetadata ActiveMetadata => MetadataLoadedEvent.Value as TemplateMetadata;

        public MetadataForm(ITemplateMetadataReader templateReader)
        {
            _templateReader = templateReader;

            MetadataLoadedEvent
               .SubscribeTo(TemplateChangedEvent.Select(x => GetMetadata(x)))
               .SubscribedBy(AddProperty<IEnumerable<KeyValuePair<InputType, string>>>(nameof(IMetadataFormState.ProjectMetadata)), metadata => BuildProjectMetadataProperties(metadata))
               .SubscribedBy(AddProperty<IEnumerable<string>>(nameof(IMetadataFormState.Dependencies)), metadata => BuildDependencyProperties(metadata));
        }

        private IEnumerable<KeyValuePair<InputType, string>> BuildProjectMetadataProperties(TemplateMetadata metadata)
        {
            var fieldIds = new List<KeyValuePair<InputType, string>>();

            if (metadata.Tags != null)
            {
                foreach (var tag in metadata.Tags)
                {
                    string name = tag.Key;
                    RemoveExistingProperty(name);

                    if (tag.Options?.Length > 0)
                    {
                        fieldIds.Add(KeyValuePair.Create(InputType.Dropdown, name));

                        if (!string.IsNullOrEmpty(tag.DefaultValue) && !tag.Options.Any(x => x == tag.DefaultValue))
                            tag.Options = tag.Options.Append(tag.DefaultValue).ToArray();

                        var prop = AddProperty(name, tag.DefaultValue ?? tag.Options.First())
                          .WithAttribute(new DropdownListAttribute
                          {
                              Label = tag.Name,
                              Placeholder = tag.Description,
                              Options = tag.Options.Select(x => KeyValuePair.Create(x, x)).ToArray()
                          });
                    }
                    else if (tag.RadioOptions?.Length > 0)
                    {
                        fieldIds.Add(KeyValuePair.Create(InputType.Radio, name));

                        if (!string.IsNullOrEmpty(tag.DefaultValue) && !tag.RadioOptions.Any(x => x == tag.DefaultValue))
                            tag.RadioOptions = tag.RadioOptions.Append(tag.DefaultValue).ToArray();

                        var prop = AddProperty(name, tag.DefaultValue ?? tag.RadioOptions.First())
                          .WithAttribute(new RadioGroupAttribute
                          {
                              Label = tag.Name,
                              Options = tag.RadioOptions.Select(x => KeyValuePair.Create(x, x)).ToArray()
                          });
                    }
                    else
                    {
                        fieldIds.Add(KeyValuePair.Create(InputType.Text, name));

                        var prop = AddProperty(name, tag.DefaultValue)
                           .WithAttribute(new TextFieldAttribute
                           {
                               Label = tag.Name,
                               Placeholder = tag.Description
                           });

                        if (tag.IsRequired)
                            prop.WithRequiredValidation();

                        if (!string.IsNullOrEmpty(tag.ValidationRegex))
                            prop.WithPatternValidation(tag.ValidationRegex, tag.ValidationError);
                    }

                    RegisterPropertyAttributes(name);
                }
            }

            return fieldIds;
        }

        private IEnumerable<string> BuildDependencyProperties(TemplateMetadata metadata)
        {
            var dependencyIds = new List<string>();

            if (metadata.ConditionalTags != null)
            {
                foreach (var tag in metadata.ConditionalTags)
                {
                    string name = tag.Key;
                    RemoveExistingProperty(name);
                    dependencyIds.Add(name);

                    AddProperty(name, tag.DefaultValue)
                       .WithAttribute(new
                       {
                           Label = tag.Name,
                           tag.Description
                       });

                    RegisterPropertyAttributes(name);
                }
            }

            return dependencyIds;
        }

        private TemplateMetadata GetMetadata(AppConfiguration.Template template)
        {
            TemplateMetadata metadata = null;
            try
            {
                metadata = _templateReader.GetMetadata(template);
            }
            catch (TemplateException) { }

            metadata ??= new TemplateMetadata();
            if (template != null)
            {
                var tags = metadata.Tags?.ToList() ?? new List<Tag>();

                if (!tags.Any(x => x.Name == TemplateMetadataReader.PROJECT_NAME || x.Key == TemplateMetadataReader.PROJECT_NAME_KEY))
                    tags.Insert(0, new Tag
                    {
                        Key = TemplateMetadataReader.PROJECT_NAME_KEY,
                        Name = TemplateMetadataReader.PROJECT_NAME,
                        DefaultValue = TemplateMetadataReader.DEFAULT_PROJECT_NAME,
                        ValidationRegex = @"^[\w\-. ]+$",
                        ValidationError = "Must be a valid filename",
                    });

                metadata.Tags = tags;
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