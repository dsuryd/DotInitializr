using System.Collections.Generic;
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
               foreach (var tag in tags.Where(x => x.Value is string).OrderByDescending(x => x.Key.Length))
               {
                  string tagPattern = tagPatterns?.ContainsKey(tag.Key) == true ? tagPatterns[tag.Key] : null;
                  if (tagPattern != null)
                  {
                     x.Name = RenderPattern(x.Name, tagPattern, tag.Value.ToString());
                     x.Content = RenderPattern(x.Content, tagPattern, tag.Value.ToString());
                  }
                  else
                  {
                     if (x.Name.Contains(tag.Key))
                        x.Name = x.Name.Replace(tag.Key, tag.Value.ToString());

                     if (x.Content.Contains(tag.Key))
                        x.Content = x.Content.Replace(tag.Key, tag.Value.ToString());
                  }
               }

               foreach (var tag in tags.Where(x => x.Value is bool))
               {
                  if (x.Name.Contains(tag.Key))
                     x.Name = x.Name.Replace(tag.Key, string.Empty);

                  bool tagValue = (bool) tag.Value;
                  string singleNewLine = "(?:\\r\\n|\\n)";
                  string newLine = $"{singleNewLine}?";

                  x.Content = RenderConditional(x.Content, $"<!--#if {tag.Key}-->(.*?)<!--#endif-->{newLine}", tagValue);
                  x.Content = RenderConditional(x.Content, $"<!--#if !{tag.Key}-->(.*?)<!--#endif-->{newLine}", !tagValue);

                  x.Content = RenderConditional(x.Content, $"\"#if {tag.Key}\": \"\"(.*?)\"#endif\": \"\",?{newLine}", tagValue);
                  x.Content = RenderConditional(x.Content, $"\"#if !{tag.Key}\": \"\"(.*?)\"#endif\": \"\",?{newLine}", !tagValue);

                  x.Content = RenderConditional(x.Content, $"#if {tag.Key}(.*?)#endif //{tag.Key}{newLine}", tagValue);
                  x.Content = RenderConditional(x.Content, $"#if !{tag.Key}(.*?)#endif //!{tag.Key}{newLine}", !tagValue);

                  x.Content = RenderConditional(x.Content, $"#if {tag.Key}{singleNewLine}(.*?)#endif{newLine}", tagValue);
                  x.Content = RenderConditional(x.Content, $"#if !{tag.Key}{singleNewLine}(.*?)#endif{newLine}", !tagValue);
               }
            }

            return new TemplateFile
            {
               Name = x.Name,
               Content = x.Content
            };
         });
      }

      private string RenderConditional(string content, string pattern, bool tagValue)
      {
         Regex regex = new Regex(pattern, RegexOptions.Singleline);
         var result = regex.Match(content);
         if (result.Success)
         {
            if (tagValue)
               return regex.Replace(content, m => m.Groups[1].Value.TrimStart('\r', '\n'));
            else
               return regex.Replace(content, string.Empty);
         }
         return content;
      }

      private string RenderPattern(string content, string pattern, string value)
      {
         Regex regex = new Regex(pattern, RegexOptions.Singleline);
         var result = regex.Match(content);
         if (result.Success && result.Groups.Count > 1)
            return regex.Replace(content, m => m.Groups[0].Value.Replace(m.Groups[1].Value, value));

         return content;
      }
   }
}