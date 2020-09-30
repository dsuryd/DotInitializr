using System;
using System.Collections.Generic;
using System.Text;

namespace DotInitialzr.Shared
{
   public interface IGeneratorState
   {
      ProjectMetadata ProjectMetadata { get; set; }
   }
}