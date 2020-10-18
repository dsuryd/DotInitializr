using System.Collections.Generic;

namespace DotInitialzr.Shared
{
   public interface IMetadataFormState
   {
      IEnumerable<string> TextFields { get; set; }
      IEnumerable<string> Checkboxes { get; set; }
   }
}