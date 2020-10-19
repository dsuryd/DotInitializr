using System.Collections.Generic;

namespace DotInitializr.Shared
{
   public class ProjectMetadata
   {
      public string ProjectName { get; set; }
      public string TemplateSourceType { get; set; }
      public string TemplateSourceUrl { get; set; }
      public string TemplateSourceDirectory { get; set; }
      public Dictionary<string, object> Tags { get; set; }
      public string FilesToExclude { get; set; }
   }
}