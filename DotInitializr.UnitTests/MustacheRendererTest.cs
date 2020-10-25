using System.Collections.Generic;
using System.Linq;
using DotInitializr.Server;
using NUnit.Framework;

namespace DotInitializr.UnitTests
{
   public class MustacheRendererTest
   {
      [Test]
      public void MustacheRenderer_RenderTag()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "file1",
               Content = "Greetings {{firstName}} {{lastName}}!"
            },
            new TemplateFile
            {
               Name = "file2",
               Content = "How can I help, {{firstName}}?"
            },
            new TemplateFile
            {
               Name = "file3",
               Content = "{{firstName:lower}} {{lastName:lower}}"
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "firstName", "John" },
            { "lastName", "Doe" },
         };

         var sut = new MustacheRenderer();
         var result = sut.Render(files, tags);

         Assert.AreEqual("Greetings John Doe!", result.FirstOrDefault(x => x.Name == "file1").Content);
         Assert.AreEqual("How can I help, John?", result.FirstOrDefault(x => x.Name == "file2").Content);
         Assert.AreEqual("john doe", result.FirstOrDefault(x => x.Name == "file3").Content);
      }

      [Test]
      public void MustacheRenderer_RenderTagOnFileName()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "{{projectName}}",
               Content = "content"
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "projectName", "StarterApp" }
         };

         var sut = new MustacheRenderer();
         var result = sut.Render(files, tags);

         Assert.IsTrue(result.Any(x => x.Name == "StarterApp"));
      }

      [Test]
      public void MustacheRenderer_RenderConditionalTag()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "file1",
               Content = "You chose {{#cond1}}One{{/cond1}}{{#cond2}}Two{{/cond2}}"
            },
            new TemplateFile
            {
               Name = "file2",
               Content = "You did not choose {{^cond1}}One{{/cond1}}{{^cond2}}Two{{/cond2}}"
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "cond1", true },
            { "cond2", false }
         };

         var sut = new MustacheRenderer();
         var result = sut.Render(files, tags);

         Assert.AreEqual("You chose One", result.FirstOrDefault(x => x.Name == "file1").Content);
         Assert.AreEqual("You did not choose Two", result.FirstOrDefault(x => x.Name == "file2").Content);
      }

      [Test]
      public void MustacheRenderer_RenderConditionalTagOnFileName()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "file{{cond1}}",
               Content = ""
            },
            new TemplateFile
            {
               Name = "file{{cond1}}/subfile",
               Content = ""
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "cond1", true }
         };

         var sut = new MustacheRenderer();
         var result = sut.Render(files, tags);

         Assert.IsTrue(result.Any(x => x.Name == "file"));
         Assert.IsTrue(result.Any(x => x.Name == "file/subfile"));
      }
   }
}