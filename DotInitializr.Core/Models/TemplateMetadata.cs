using System.Collections.Generic;

namespace DotInitializr
{
   public class Tag
   {
      public string Key { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public string DefaultValue { get; set; }
      public string Regex { get; set; }
      public string ValidationRegex { get; set; }
      public string ValidationError { get; set; }
      public string[] Options { get; set; }
      public string[] RadioOptions { get; set; }
      public string DataType { get; set; }
      public bool IsRequired { get; set; } = true;
   }

   public class ConditionalTag
   {
      public string Key { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public bool? DefaultValue { get; set; }
      public string FilesToInclude { get; set; }
   }

   public class ComputedTag
   {
      public string Key { get; set; }
      public string Expression { get; set; }
      public string FilesToInclude { get; set; }
   }

   public class TemplateMetadata
   {
      public const string FILE_NAME = "dotInitializr.json";

      public string TemplateType { get; set; } = DotNetRenderer.TEMPLATE_TYPE;

      public List<Tag> Tags { get; set; }
      public List<ConditionalTag> ConditionalTags { get; set; }
      public List<ComputedTag> ComputedTags { get; set; }
   }
}