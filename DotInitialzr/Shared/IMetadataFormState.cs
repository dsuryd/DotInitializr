using System.Collections.Generic;

namespace DotInitialzr.Shared
{
   public interface IMetadataFormState
   {
      string ProjectName { get; set; }
      IEnumerable<string> TextFields { get; set; }
      IEnumerable<string> Checkboxes { get; set; }
   }
}