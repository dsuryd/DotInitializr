using System;
using System.Collections.Generic;
using System.Text;

namespace DotInitialzr.Shared
{
   public interface IGeneratorState
   {
      TemplateMetadata TemplateMetadata { get; set; }
   }
}