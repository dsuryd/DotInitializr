using System.Collections.Generic;

namespace DotInitializr.Server
{
   public interface ITemplateRenderer
   {
      IEnumerable<TemplateFile> Render(IEnumerable<TemplateFile> files, Dictionary<string, object> tags);
   }
}