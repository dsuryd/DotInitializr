using System.Collections.Generic;

namespace DotInitialzr.Server
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
      public IEnumerable<string> FilesToInclude { get; set; }
   }

   public class TemplateMetadata
   {
      public IEnumerable<TextTemplateTag> TextTags { get; set; }
      public IEnumerable<ConditionalTemplateTag> ConditionalTags { get; set; }
   }
}