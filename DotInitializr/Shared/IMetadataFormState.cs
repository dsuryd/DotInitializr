using System.Collections.Generic;

namespace DotInitializr.Shared
{
   public interface IMetadataFormState
   {
      IEnumerable<string> TextFields { get; set; }
      IEnumerable<string> Checkboxes { get; set; }
   }
}