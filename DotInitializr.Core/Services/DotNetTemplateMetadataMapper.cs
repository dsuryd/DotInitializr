using System;
using System.Collections.Generic;
using System.Linq;

namespace DotInitializr
{
    public class DotNetTemplateMetadataMapper
    {
        public TemplateMetadata Map(DotNetTemplateMetadata dotNetMetadata)
        {
            var tags = new List<Tag>();
            var conditionalTags = new List<ConditionalTag>();
            var computedTags = new List<ComputedTag>();

            tags.Add(new Tag
            {
                Key = dotNetMetadata.SourceName,
                Name = "Project Name",
                DefaultValue = dotNetMetadata.SourceName
            });

            foreach (var symbol in dotNetMetadata.Symbols)
            {
                var generator = symbol.Value;
                string dataType = generator.Datatype?.ToString();

                if (generator.Type == SymbolType.Parameter)
                {
                    if (dataType == "bool")
                        conditionalTags.Add(BuildConditionalTag(symbol.Key, generator));
                    else if (dataType == "string")
                        tags.Add(BuildInputTag(symbol.Key, generator));
                    else if (dataType == "choice")
                        tags.Add(BuildOptionsTag(symbol.Key, generator));
                }
                else if (generator.Type == SymbolType.Computed)
                {
                    computedTags.Add(BuildComputedTag(symbol.Key, generator));
                }
                else if (generator.Type == SymbolType.Derived)
                {
                    computedTags.Add(BuildComputedTagFromDerived(symbol.Key, generator));
                }
            }

            foreach (var source in dotNetMetadata.Sources)
            {
                foreach (var modifier in source.Modifiers)
                {
                    ConditionalTag conditionalTag = conditionalTags.Find(x => modifier.Condition.Contains(x.Key));
                    if (conditionalTag != null)
                    {
                        if (modifier.Condition.Contains($"!{conditionalTag.Key}"))
                        {
                            if (modifier.Exclude != null)
                                conditionalTag.FilesToInclude = GetFilesToExclude(modifier);
                        }
                        else
                        {
                            if (modifier.Include != null)
                                conditionalTag.FilesToInclude = GetFilesToInclude(modifier);
                        }
                    }

                    ComputedTag computedTag = computedTags.Find(x => modifier.Condition.Contains(x.Key));
                    if (computedTag != null)
                    {
                        if (modifier.Condition.Contains($"!{computedTag.Key}"))
                        {
                            if (modifier.Exclude != null)
                                computedTag.FilesToInclude = GetFilesToExclude(modifier);
                        }
                        else
                        {
                            if (modifier.Include != null)
                                computedTag.FilesToInclude = GetFilesToInclude(modifier);
                        }
                    }
                }
            }

            return new TemplateMetadata
            {
                TemplateType = "dotnet",
                Tags = tags,
                ConditionalTags = conditionalTags,
                ComputedTags = computedTags
            };
        }

        private Tag BuildInputTag(string symbolKey, Generator generator)
        {
            return new Tag
            {
                Key = symbolKey,
                Name = symbolKey,
                Regex = generator.Replaces,
                Description = generator.Description,
                DefaultValue = generator.DefaultValue
            };
        }

        private Tag BuildOptionsTag(string symbolKey, Generator generator)
        {
            var options = generator.Choices.Select(x => x.ChoiceClass?.Choice).ToArray();
            return new Tag
            {
                Key = symbolKey,
                Name = symbolKey,
                Regex = generator.Replaces,
                Description = generator.Description,
                RadioOptions = options.Length == 2 ? options : null,
                Options = options.Length > 2 ? options : null,
                DefaultValue = generator.DefaultValue
            };
        }

        private ConditionalTag BuildConditionalTag(string symbolKey, Generator generator)
        {
            return new ConditionalTag
            {
                Key = symbolKey,
                Name = symbolKey,
                Description = generator.Description,
                DefaultValue = generator.DefaultValue.Equals("true", StringComparison.InvariantCultureIgnoreCase)
            };
        }

        private ComputedTag BuildComputedTag(string symbolKey, Generator generator)
        {
            return new ComputedTag
            {
                Key = symbolKey,
                Expression = generator.Value
            };
        }

        private ComputedTag BuildComputedTagFromDerived(string symbolKey, Generator generator)
        {
            return new ComputedTag
            {
                Key = generator.Replaces ?? symbolKey,
                Expression = $"{generator.ValueTransform}({generator.ValueSource})"
            };
        }

        private string GetFilesToExclude(Modifier modifier)
        {
            if (modifier.Exclude.Value.String != null)
                return modifier.Exclude.Value.String;
            else if (modifier.Exclude.Value.StringArray != null)
                return string.Join(',', modifier.Exclude.Value.StringArray);
            return null;
        }

        private string GetFilesToInclude(Modifier modifier)
        {
            if (modifier.Include.Value.String != null)
                return modifier.Include.Value.String;
            else if (modifier.Include.Value.StringArray != null)
                return string.Join(',', modifier.Include.Value.StringArray);
            return null;
        }
    }
}