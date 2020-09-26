using System.Collections.Generic;

namespace DotInitialzr.Server
{
   public interface ITemplateSource
   {
      string SourceType { get; }

      IEnumerable<TemplateFile> GetFiles(string sourceUrl, string sourceDirectory = null);
   }
}