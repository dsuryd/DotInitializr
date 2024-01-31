using System;
using System.Collections.Generic;

namespace DotInitializr
{
	public class AppConfigurationV2
	{
		internal static readonly string SECTION = "DotInitializrV2";

		public class Template
		{
			public string Key { get; } = Guid.NewGuid().ToString();
			public string Name { get; set; }
			public string Description { get; set; }
			public string SourceType { get; set; } = "git";
			public string SourceUrl { get; set; }
			public string SourceBranch { get; set; }
			public string SourceDirectory { get; set; }
			public string TemplateType { get; set; } = "dotinitializr";
		}

		public IEnumerable<Template> Templates { get; set; }
		public string UserName { get; set; } = null;
		public string PersonalAccessToken { get; set; } = null;
	}
}