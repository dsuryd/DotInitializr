using System;
using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Builders;

namespace DotInitializr.Server
{
   public class MustacheRenderer : ITemplateRenderer
   {
      public IEnumerable<TemplateFile> Render(IEnumerable<TemplateFile> files, Dictionary<string, object> tags)
      {
         var conditionalTags = tags.Where(x => x.Value is bool)?.Select(x => x.Key);

         try
         {
            var stubble = new StubbleBuilder().Build();
            return files.Select(x =>
            {
               if (conditionalTags != null)
               {
                  foreach (var tag in conditionalTags)
                     x.Name = x.Name.Replace("{{" + tag + "}}", string.Empty);
               }

               return new TemplateFile
               {
                  Name = stubble.Render(x.Name, tags),
                  Content = stubble.Render(x.Content, tags)
               };
            });
         }
         catch (Exception ex)
         {
            throw new TemplateException(ex.Message);
         }
      }
   }
}