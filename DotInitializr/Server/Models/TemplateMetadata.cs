using System.Collections.Generic;

namespace DotInitializr.Server
{
   public class TextTemplateTag
   {
      public string Key { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public string DefaultValue { get; set; }
      public string ValidationRegex { get; set; }
      public string ValidationError { get; set; }
   }

   public class ConditionalTemplateTag
   {
      public string Key { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public bool? DefaultValue { get; set; }
      public string FilesToInclude { get; set; }
   }

   public class ComputedTemplateTag
   {
      public string Key { get; set; }
      public string Expression { get; set; }
   }

   public class TemplateMetadata
   {
      public const string FILE_NAME = "dotInitializr.json";

      public IEnumerable<TextTemplateTag> TextTags { get; set; }
      public IEnumerable<ConditionalTemplateTag> ConditionalTags { get; set; }
      public IEnumerable<ComputedTemplateTag> ComputedTags { get; set; }
   }
}