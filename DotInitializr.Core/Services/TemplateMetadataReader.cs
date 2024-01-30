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

using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DotInitializr
{
	/// <summary>
	/// Reads metadata from a project template.
	/// </summary>
	public interface ITemplateMetadataReader
	{
		TemplateMetadata GetMetadata(AppConfiguration.Template template);

		Dictionary<string, object> GetComputedTags(TemplateMetadata metadata, Dictionary<string, object> tagValues);

		Dictionary<string, bool> GetConditionalTags(TemplateMetadata metadata);

		Dictionary<string, object> GetTags(TemplateMetadata metadata);

		Dictionary<string, string> GetTagRegexes(TemplateMetadata metadata);

		string GetFilesToExclude(TemplateMetadata metadata, Dictionary<string, bool> conditionalTags);
	}

	public class TemplateMetadataReader : ITemplateMetadataReader
	{
		public const string PROJECT_NAME_KEY = "projectName";
		public const string PROJECT_NAME = "Project Name";
		public const string DEFAULT_PROJECT_NAME = "Starter";

		private readonly IEnumerable<IConfigurableTemplateSource> _templateSources;

		private delegate int CountDelegate(params bool[] tags);

		private delegate string TransformDelegate(string value);

		public TemplateMetadataReader(IEnumerable<IConfigurableTemplateSource> templateSources)
		{
			_templateSources = templateSources;
		}

		public TemplateMetadata GetMetadata(AppConfiguration.Template template)
		{
			TemplateMetadata metadata = null;

			var metadataFile = GetMetadataFile(template);
			if (metadataFile != null && !string.IsNullOrEmpty(metadataFile.Content))
			{
				try
				{
					if (metadataFile.Name.EndsWith(DotNetTemplateMetadata.FILE_NAME, StringComparison.InvariantCultureIgnoreCase))
						metadata = new DotNetTemplateMetadataMapper().Map(metadataFile.Content.ToDotNetTemplateMetadata());
					else
						metadata = JsonSerializer.Deserialize<TemplateMetadata>(metadataFile.Content);
				}
				catch (Exception ex)
				{
					var error = $"`{metadataFile.Name}` in `{template.SourceUrl}` must be in JSON";
					Console.WriteLine(error + Environment.NewLine + ex.ToString());
					throw new TemplateException(error, ex);
				}

				// Make sure the tags have keys. Names can be used to substitute keys.
				if (metadata.Tags != null)
				{
					foreach (var tag in metadata.Tags.Where(x => string.IsNullOrEmpty(x.Key)))
						tag.Key = tag.Name;
					metadata.Tags = metadata.Tags.Where(x => !string.IsNullOrEmpty(x.Key));
				}

				if (metadata.ConditionalTags != null)
				{
					foreach (var tag in metadata.ConditionalTags.Where(x => string.IsNullOrEmpty(x.Key)))
						tag.Key = tag.Name;
					metadata.ConditionalTags = metadata.ConditionalTags.Where(x => !string.IsNullOrEmpty(x.Key));
				}

				if (metadata.ComputedTags != null)
					metadata.ComputedTags = metadata.ComputedTags.Where(x => !string.IsNullOrEmpty(x.Key));
			}

			return metadata;
		}

		private TemplateFile GetMetadataFile(AppConfiguration.Template template)
		{
			var templateSource = _templateSources.FirstOrDefault(x => string.Equals(x.SourceType, template?.SourceType, StringComparison.InvariantCultureIgnoreCase));
			if (templateSource != null)
			{
				if (template.TemplateType?.ToLower() == "dotnet")
					return templateSource.GetFile(DotNetTemplateMetadata.FILE_NAME, template.SourceUrl, template.SourceDirectory + DotNetTemplateMetadata.FILE_PATH, template.SourceBranch);
				else
					return templateSource.GetFile(TemplateMetadata.FILE_NAME, template.SourceUrl, template.SourceDirectory, template.SourceBranch);
			}

			return null;
		}

		public Dictionary<string, object> GetComputedTags(TemplateMetadata metadata, Dictionary<string, object> tagValues)
		{
			var result = new Dictionary<string, object>();

			if (metadata.ComputedTags != null)
			{
				var interpreter = new Interpreter();
				foreach (var tag in tagValues)
					interpreter.SetVariable(tag.Key, tag.Value);

				CountDelegate countFunc = Count;
				TransformDelegate xmlEncodeFunc = XmlEncode;
				TransformDelegate lowerCaseFunc = LowerCase;
				TransformDelegate upperCaseFunc = UpperCase;
				TransformDelegate titleCaseFunc = TitleCase;
				TransformDelegate kebabCaseFunc = KebabCase;
				interpreter.SetFunction("Count", countFunc);
				interpreter.SetFunction("count", countFunc);
				interpreter.SetFunction("xmlEncode", xmlEncodeFunc);
				interpreter.SetFunction("lowerCase", lowerCaseFunc);
				interpreter.SetFunction("lowerCaseInvariant", lowerCaseFunc);
				interpreter.SetFunction("upperCase", upperCaseFunc);
				interpreter.SetFunction("upperCaseInvariant", upperCaseFunc);
				interpreter.SetFunction("titleCase", titleCaseFunc);
				interpreter.SetFunction("kebabCase", kebabCaseFunc);

				foreach (var computedTag in metadata.ComputedTags)
				{
					try
					{
						if (!result.ContainsKey(computedTag.Key))
						{
							object value = interpreter.Eval(computedTag.Expression);
							object computedValue = value is bool ? ((bool)value) == true : value;
							result.Add(computedTag.Key, computedValue);
							if (!tagValues.ContainsKey(computedTag.Key))
								interpreter.SetVariable(computedTag.Key, computedValue);
						}
					}
					catch (Exception ex)
					{
						var error = $"Cannot compute `{computedTag.Key}` expression `{computedTag.Expression}: {ex}`";
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(error);
						Console.ResetColor();
					}
				}
			}

			return result;
		}

		public Dictionary<string, object> GetTags(TemplateMetadata metadata)
		{
			var result = new Dictionary<string, object>();

			if (metadata?.Tags != null)
				foreach (var tag in metadata.Tags)
				{
					if (tag.DataType == nameof(Int32))
						result.Add(tag.Key, string.IsNullOrWhiteSpace(tag.DefaultValue) ? default : int.Parse(tag.DefaultValue));
					else if (tag.DataType == nameof(Single))
						result.Add(tag.Key, string.IsNullOrWhiteSpace(tag.DefaultValue) ? default : float.Parse(tag.DefaultValue));
					else
						result.Add(tag.Key, tag.DefaultValue ?? tag.Regex);
				}

			return result;
		}

		public Dictionary<string, string> GetTagRegexes(TemplateMetadata metadata)
		{
			var result = new Dictionary<string, string>();

			if (metadata?.Tags != null)
				foreach (var tag in metadata.Tags.Where(x => !string.IsNullOrWhiteSpace(x.Regex)))
					result.Add(tag.Key, tag.Regex);

			return result;
		}

		public Dictionary<string, bool> GetConditionalTags(TemplateMetadata metadata)
		{
			var result = new Dictionary<string, bool>();

			if (metadata?.ConditionalTags != null)
				foreach (var tag in metadata.ConditionalTags)
					result.Add(tag.Key, tag.DefaultValue ?? false);

			return result;
		}

		public string GetFilesToExclude(TemplateMetadata metadata, Dictionary<string, bool> tags)
		{
			string result = "";

			if (metadata.ConditionalTags != null)
				foreach (var tag in metadata.ConditionalTags.Where(x => !tags.ContainsKey(x.Key) || !tags[x.Key]))
					result = string.Join(',', result, tag.FilesToInclude);

			if (metadata.ComputedTags != null)
				foreach (var tag in metadata.ComputedTags.Where(x => !tags.ContainsKey(x.Key) || !tags[x.Key]))
					result = string.Join(',', result, tag.FilesToInclude);

			return result.Trim(',');
		}

		private int Count(params bool[] tags) => tags.Where(x => x).Count();

		private string XmlEncode(string value) => System.Security.SecurityElement.Escape(value);

		private string LowerCase(string value) => value?.ToLower();

		private string UpperCase(string value) => value?.ToUpper();

		private string TitleCase(string value) => new CultureInfo("en-US", false).TextInfo.ToTitleCase(value);

		private string KebabCase(string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			return Regex.Replace(
				value,
				"(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
				"-$1",
				RegexOptions.Compiled)
				.Trim()
				.ToLower();
		}
	}
}