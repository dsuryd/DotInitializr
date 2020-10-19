using System.Collections.Generic;

namespace DotInitializr.Server
{
   public interface ITemplateSource
   {
      string SourceType { get; }

      TemplateFile GetFile(string fileName, string sourceUrl, string sourceDirectory = null);

      IEnumerable<TemplateFile> GetFiles(string sourceUrl, string sourceDirectory = null);
   }
}