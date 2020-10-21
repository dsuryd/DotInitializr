using System.Collections.Generic;

namespace DotInitializr.Shared
{
   public enum InputType
   {
      Text,
      Dropdown,
      Radio
   }

   public interface IMetadataFormState
   {
      IEnumerable<KeyValuePair<InputType, string>> ProjectMetadata { get; set; }
      IEnumerable<string> Dependencies { get; set; }
   }
}