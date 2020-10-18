using System.Collections.Generic;

namespace DotInitialzr.Shared
{
   public interface IGeneratorFormState
   {
      string Template { get; set; }
      Dictionary<string, string> Generate { get; set; }
      bool ShowMetadataForm { get; set; }
      ProjectMetadata ProjectMetadata { get; set; }
   }
}