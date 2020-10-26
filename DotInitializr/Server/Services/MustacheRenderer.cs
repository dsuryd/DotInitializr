/*
Copyright 2020 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Stubble.Core.Builders;

namespace DotInitializr.Server
{
   /// <summary>
   /// Renders a set of project files from a project template and metadata that uses Mustache notations (http://mustache.github.io/mustache.5.html).
   /// </summary>
   public class MustacheRenderer : ITemplateRenderer
   {
      public const string TEMPLATE_TYPE = "mustache";
      private const string LOWER_CASE = "__lower";
      private const string UPPER_CASE = "__upper";

      public string TemplateType => TEMPLATE_TYPE;

      public IEnumerable<TemplateFile> Render(IEnumerable<TemplateFile> files, Dictionary<string, object> tags)
      {
         var conditionalTags = tags.Where(x => x.Value is bool)?.Select(x => x.Key);

         var filesWithFormat = files.Where(x => x.Content.Contains(LOWER_CASE) || x.Content.Contains(UPPER_CASE) || x.Name.Contains(LOWER_CASE) || x.Name.Contains(UPPER_CASE));
         foreach (var tag in tags.Where(x => x.Value is string && filesWithFormat.Any(y => y.Content.Contains($"{x.Key}__") || y.Name.Contains($"{x.Key}__"))).ToArray())
         {
            tags.Add($"{tag.Key}{LOWER_CASE}", tag.Value.ToString().ToLowerInvariant());
            tags.Add($"{tag.Key}{UPPER_CASE}", tag.Value.ToString().ToUpperInvariant());
         }

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