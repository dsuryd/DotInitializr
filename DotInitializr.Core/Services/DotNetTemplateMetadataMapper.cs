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

            if (!string.IsNullOrWhiteSpace(dotNetMetadata.SourceName))
            {
                tags.Add(new Tag
                {
                    Key = TemplateMetadataReader.PROJECT_NAME_KEY,
                    Name = TemplateMetadataReader.PROJECT_NAME,
                    DefaultValue = dotNetMetadata.SourceName,
                    Regex = dotNetMetadata.SourceName
                });
            }

            foreach (var symbol in dotNetMetadata.Symbols)
            {
                var generator = symbol.Value;
                string dataType = generator.Datatype?.ToString();

                if (generator.Type == SymbolType.Parameter)
                {
                    if (string.Compare(dataType, "bool", true) == 0)
                        conditionalTags.Add(BuildConditionalTag(symbol.Key, generator));
                    else if (string.Compare(dataType, "choice", true) == 0)
                        tags.Add(BuildOptionsTag(symbol.Key, generator));
                    else
                        tags.Add(BuildInputTag(symbol.Key, generator));
                }
                else if (generator.Type == SymbolType.Computed)
                {
                    computedTags.Add(BuildComputedTag(symbol.Key, generator));
                }
                else if (generator.Type == SymbolType.Derived)
                {
                    computedTags.AddRange(BuildComputedTagFromDerived(symbol.Key, generator));
                }
                else if (generator.Type == SymbolType.Generated)
                {
                    computedTags.AddRange(BuildComputedTagFromGenerated(symbol.Key, generator));
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
            var dataType = generator.Datatype?.ToString();
            return new Tag
            {
                Key = symbolKey,
                Name = symbolKey,
                Regex = generator.Replaces,
                Description = generator.Description,
                DefaultValue = generator.DefaultValue,
                DataType =
                    string.Compare(dataType, "int", true) == 0 || string.Compare(dataType, "integer", true) == 0 ? nameof(Int32)
                    : string.Compare(dataType, "float", true) == 0 ? nameof(Single)
                    : null
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

        private ComputedTag[] BuildComputedTagFromDerived(string symbolKey, Generator generator)
        {
            var result = new ComputedTag
            {
                Key = symbolKey,
                Expression = $"{generator.ValueTransform}({generator.ValueSource})"
            };

            if (!string.IsNullOrWhiteSpace(generator.Replaces))
                return new[]
                {
                    result,
                    new ComputedTag
                    {
                        Key = generator.Replaces,
                        Expression = result.Expression
                    }
                };

            return new[] { result };
        }

        private ComputedTag[] BuildComputedTagFromGenerated(string symbolKey, Generator generator)
        {
            ComputedTag result = null;

            if (generator.GeneratorGenerator == GeneratorEnum.Constant)
            {
                if (generator.Parameters.HasValue)
                {
                    result = new ComputedTag
                    {
                        Key = symbolKey,
                        Expression = generator.Parameters.Value.ParametersClass.Value
                    };
                }
            }
            else if (generator.GeneratorGenerator == GeneratorEnum.Casing)
            {
                if (generator.Parameters.HasValue)
                {
                    var valueTransform = generator.Parameters.Value.ParametersClass.ToLower == true ? "lowerCase" : "upperCase";
                    result = new ComputedTag
                    {
                        Key = symbolKey,
                        Expression = $"{valueTransform}({generator.Parameters.Value.ParametersClass.Source})"
                    };
                }
            }
            else if (generator.GeneratorGenerator == GeneratorEnum.Coalesce)
            {
                if (generator.Parameters.HasValue)
                {
                    var parameters = generator.Parameters.Value.ParametersClass;
                    result = new ComputedTag
                    {
                        Key = symbolKey,
                        Expression = $"{parameters.SourceVariableName} != null ? {parameters.SourceVariableName} : {parameters.FallbackVariableName}"
                    };
                }
            }
            else if (generator.GeneratorGenerator == GeneratorEnum.Evaluate)
            {
                if (generator.Parameters.HasValue)
                {
                    var parameters = generator.Parameters.Value.ParametersClass;
                    result = new ComputedTag
                    {
                        Key = symbolKey,
                        Expression = parameters.Action
                    };
                }
            }
            else if (generator.GeneratorGenerator == GeneratorEnum.Guid)
            {
                if (generator.Parameters.HasValue)
                {
                    var parameters = generator.Parameters.Value.ParametersClass;
                    result = new ComputedTag
                    {
                        Key = symbolKey,
                        Expression = Guid.NewGuid().ToString(parameters.Format ?? "")
                    };
                }
            }
            else if (generator.GeneratorGenerator == GeneratorEnum.Port)
            {
                if (generator.Parameters.HasValue)
                {
                    var parameters = generator.Parameters.Value.ParametersClass;
                    result = new ComputedTag
                    {
                        Key = symbolKey,
                        Expression = $"{parameters.Fallback}"
                    };
                }
            }
            else if (generator.GeneratorGenerator == GeneratorEnum.Now)
            {
                if (generator.Parameters.HasValue)
                {
                    var parameters = generator.Parameters.Value.ParametersClass;
                    result = new ComputedTag
                    {
                        Key = symbolKey,
                        Expression = parameters.Utc == true ? DateTime.UtcNow.ToString(parameters.Format) : DateTime.Now.ToString(parameters.Format)
                    };
                }
            }
            else if (generator.GeneratorGenerator == GeneratorEnum.Random)
            {
                if (generator.Parameters.HasValue)
                {
                    var parameters = generator.Parameters.Value.ParametersClass;
                    result = new ComputedTag
                    {
                        Key = symbolKey,
                        Expression = $"{new Random().Next((int)parameters.Low, (int)parameters.High)}"
                    };
                }
            }
            else if (generator.GeneratorGenerator == GeneratorEnum.Regex)
            {
                if (generator.Parameters.HasValue)
                {
                    var parameters = generator.Parameters.Value.ParametersClass;
                    result = new ComputedTag
                    {
                        Key = symbolKey,
                        Expression = parameters.Utc == true ? DateTime.UtcNow.ToString(parameters.Format) : DateTime.Now.ToString(parameters.Format)
                    };
                }
            }
            else
                throw new NotImplementedException();

            if (!string.IsNullOrWhiteSpace(generator.Replaces))
                return new[]
                {
                    result,
                    new ComputedTag
                    {
                        Key = generator.Replaces,
                        Expression = result.Expression
                    }
                };

            return new[] { result };
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