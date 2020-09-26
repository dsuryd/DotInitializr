using System;
using System.Collections.Generic;

namespace DotInitialzr.Server
{
   public class InitialzrConfiguration
   {
      internal static readonly string SECTION = "DotInitialzr";

      public class Template
      {
         public string Key { get; } = Guid.NewGuid().ToString();
         public string Name { get; set; }
         public string Description { get; set; }
         public string SourceType { get; set; }
         public string SourceUrl { get; set; }
         public string SourceDirectory { get; set; }
      }

      public IEnumerable<Template> Templates { get; set; }
   }
}