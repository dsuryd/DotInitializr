using System.Collections.Generic;

namespace DotInitialzr.Server
{
   public interface ITemplateRenderer
   {
      IEnumerable<TemplateFile> Render(IEnumerable<TemplateFile> files, Dictionary<string, object> tags);
   }
}