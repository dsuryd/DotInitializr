using System.Collections.Generic;

namespace DotInitialzr.Shared
{
   public interface IGeneratorFormState
   {
      bool Loading { get; set; }
      ProjectMetadata ProjectMetadata { get; set; }

      void ClearProjectMetadata();
   }
}