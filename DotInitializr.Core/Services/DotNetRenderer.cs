﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotInitializr
{
   public class DotNetRenderer : ITemplateRenderer
   {
      public const string TEMPLATE_TYPE = "dotnet";
      private const string LOWER_CASE = "__lower";
      private const string UPPER_CASE = "__upper";

      public string TemplateType => TEMPLATE_TYPE;

      public IEnumerable<TemplateFile> Render(IEnumerable<TemplateFile> files, Dictionary<string, object> tags) => Render(files, tags, null);

      public IEnumerable<TemplateFile> Render(IEnumerable<TemplateFile> files, Dictionary<string, object> tags, Dictionary<string, string> tagPatterns)
      {
         // Add tag lower-case and upper-case support.
         var filesWithFormat = files.Where(x => x.Content.Contains(LOWER_CASE) || x.Content.Contains(UPPER_CASE) || x.Name.Contains(LOWER_CASE) || x.Name.Contains(UPPER_CASE));
         foreach (var tag in tags.Where(x => x.Value is string && filesWithFormat.Any(y => y.Content.Contains($"{x.Key}__") || y.Name.Contains($"{x.Key}__"))).ToArray())
         {
            tags.Add($"{tag.Key}{LOWER_CASE}", tag.Value.ToString().ToLowerInvariant());
            tags.Add($"{tag.Key}{UPPER_CASE}", tag.Value.ToString().ToUpperInvariant());
         }

         return files.Select(x =>
         {
            if (tags != null)
            {
               x.Content = RenderConditional(x.Content, tags, "<!--", "-->\\s*"); // for XML/HTML.
               x.Content = RenderConditional(x.Content, tags, @"""", @""": """",?"); // for JSON.
               x.Content = RenderConditional(x.Content, tags);

               x.Content = RenderElifConditional(x.Content, tags, "<!--", "-->\\s*"); // for XML/HTML.
               x.Content = RenderElifConditional(x.Content, tags, @"""", @""": """",?"); // for JSON.
               x.Content = RenderElifConditional(x.Content, tags);

               // Do another pass to resolve nested conditionals.
               x.Content = RenderConditional(x.Content, tags, "<!--", "-->\\s*"); // for XML/HTML.
               x.Content = RenderConditional(x.Content, tags, @"""", @""": """",?"); // for JSON.
               x.Content = RenderConditional(x.Content, tags);

               foreach (var tag in tags.Where(x => !(x.Value is bool) && x.Value != null).OrderByDescending(x => x.Key.Length))
               {
                  string tagValue = $"{tag.Value}";
                  string tagPattern = tagPatterns?.ContainsKey(tag.Key) == true ? tagPatterns[tag.Key] : null;
                  if (tagPattern != null)
                  {
                     x.Name = RenderPattern(x.Name, tagPattern, tagValue);
                     x.Content = RenderPattern(x.Content, tagPattern, tagValue);
                  }
                  else
                  {
                     if (x.Name.Contains(tag.Key))
                        x.Name = x.Name.Replace(tag.Key, tagValue);

                     if (x.Content.Contains(tag.Key))
                        x.Content = x.Content.Replace(tag.Key, tagValue);
                  }
               }
            }

            return x is TemplateFileBinary
                ? new TemplateFileBinary { Name = x.Name, ContentBytes = (x as TemplateFileBinary).ContentBytes }
                : new TemplateFile { Name = x.Name, Content = x.Content };
         });
      }

      private string RenderConditional(string content, Dictionary<string, object> tags, string openTag = "", string closeTag = "")
      {
         string singleNewLine = @"(?:\t*\r\n|\t*\n)";
         string newLine = $"{singleNewLine}?";
         string comment = @"(?://)?";
         string tagPattern = @"\(?((?:(?!\n).)*?)\)?";
         string bodyPattern = @"((?:(?!#if|#elif|#elseif).)*?)";

         string regexPattern = $@"{comment}{openTag}#if {tagPattern}{closeTag}{singleNewLine}{bodyPattern}(?:{comment}{openTag}#else{closeTag}{singleNewLine}{bodyPattern})?{comment}{openTag}#endif{closeTag}{newLine}";
         Regex regex = new Regex(regexPattern, RegexOptions.Singleline);
         bool updated = false;
         Match result;

         do
         {
            result = regex.Match(content);
            if (result.Success)
            {
               updated = false;
               content = regex.Replace(content, m =>
               {
                  bool negation = false;
                  string key = m.Groups[1].Value.Trim('\r', '\n');
                  if (key.StartsWith("!"))
                  {
                     key = key.TrimStart('!');
                     negation = true;
                  }

                  string body = m.Groups[2].Value.TrimStart('\r', '\n');
                  string elseBody = m.Groups.Count == 4 ? m.Groups[3].Value.TrimStart('\r', '\n') : string.Empty;

                  updated = true;
                  if (tags.ContainsKey(key) && tags[key] is bool value)
                     return value ^ negation ? body : elseBody;
                  else
                     return elseBody;
               });
            }
         }
         while (result.Success && updated);
         return content;
      }

      private string RenderElifConditional(string content, Dictionary<string, object> tags, string openTag = "", string closeTag = "")
      {
         string singleNewLine = @"(?:\r\n|\n)";
         string newLine = $"{singleNewLine}?";
         string comment = @"(?://)?";
         string tagPattern = @"\(?((?:(?!\n).)*?)\)?";
         string bodyPattern = @"((?:(?!#if|#elif|#elseif).)*?)";

         string regexPattern = $@"{comment}{openTag}#if {tagPattern}{closeTag}{singleNewLine}{bodyPattern}(?:{comment}{openTag}(?:#elif|#elseif) {tagPattern}{closeTag}{singleNewLine}{bodyPattern})+{comment}{openTag}#endif{closeTag}{newLine}";
         Regex regex = new Regex(regexPattern, RegexOptions.Singleline);
         bool updated = false;
         Match result;

         do
         {
            result = regex.Match(content);
            if (result.Success)
            {
               updated = true;
               content = regex.Replace(content, m =>
               {
                  int i = 1;
                  for (i = 1; i < m.Groups.Count; i += 2)
                  {
                     bool negation = false;
                     var key = m.Groups[i].Value.Trim('\r', '\n');
                     if (key.StartsWith("!"))
                     {
                        key = key.TrimStart('!');
                        negation = true;
                     }

                     var body = m.Groups[i + 1].Value.TrimStart('\r', '\n');

                     if (tags.ContainsKey(key) && tags[key] is bool value)
                     {
                        if (value ^ negation)
                           return body;
                     }
                     else
                     {
                        updated = false;
                        return m.Groups[0].Value;
                     }
                  }

                  return string.Empty;
               });
            }
         }
         while (result.Success && updated);
         return content;
      }

      private string RenderPattern(string content, string pattern, string value)
      {
         Regex regex = new Regex(pattern, RegexOptions.Singleline);
         var result = regex.Match(content);
         if (result.Success)
            if (result.Groups.Count > 1)
               return regex.Replace(content, m => m.Groups[0].Value.Replace(m.Groups[1].Value, value));
            else
               return regex.Replace(content, value);

         return content;
      }
   }
}