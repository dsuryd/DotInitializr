using System.Collections.Generic;

namespace DotInitializr.Server
{
   public class Tag
   {
      public string Key { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public string DefaultValue { get; set; }
      public string ValidationRegex { get; set; }
      public string ValidationError { get; set; }
      public string[] Options { get; set; }
      public string[] RadioOptions { get; set; }
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

      public IEnumerable<Tag> Tags { get; set; }
      public IEnumerable<ConditionalTag> ConditionalTags { get; set; }
      public IEnumerable<ComputedTag> ComputedTags { get; set; }
   }
}