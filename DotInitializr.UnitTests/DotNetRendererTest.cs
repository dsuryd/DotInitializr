using System.Collections.Generic;
using System.Linq;
using DotInitializr.Website.Server;
using NUnit.Framework;

namespace DotInitializr.UnitTests
{
   public class DotNetRendererTest
   {
      [Test]
      public void DotNetRenderer_RenderTag()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "file1",
               Content = "Greetings firstName lastName!"
            },
            new TemplateFile
            {
               Name = "file2",
               Content = "How can I help, firstName?"
            },
            new TemplateFile
            {
               Name = "file3__upper",
               Content = "firstName__lower lastName__upper"
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "firstName", "John" },
            { "lastName", "Doe" },
            { "file3", "filename3" }
         };

         var sut = new DotNetRenderer();
         var result = sut.Render(files, tags);

         Assert.AreEqual("Greetings John Doe!", result.FirstOrDefault(x => x.Name == "file1").Content);
         Assert.AreEqual("How can I help, John?", result.FirstOrDefault(x => x.Name == "file2").Content);
         Assert.AreEqual("john DOE", result.FirstOrDefault(x => x.Name == "FILENAME3").Content);
      }

      [Test]
      public void DotNetRenderer_RenderTagOnFileName()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "projectName",
               Content = "content"
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "projectName", "StarterApp" }
         };

         var sut = new DotNetRenderer();
         var result = sut.Render(files, tags);

         Assert.IsTrue(result.Any(x => x.Name == "StarterApp"));
      }

      [Test]
      public void DotNetRenderer_RenderConditionalTag()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "file1",
               Content = @"You chose
#if cond1
One
#endif
#if cond2
Two
#endif"
            },
            new TemplateFile
            {
               Name = "file2",
               Content = @"You did not choose
#if !cond1
One
#endif
#if !cond2
Two
#endif"
            },
            new TemplateFile
            {
               Name = "file3",
               Content = $@"You chose
<!--#if cond1-->
One
<!--#endif-->
not
<!--#if !cond2-->
Two
<!--#endif-->"
            } };

         var tags = new Dictionary<string, object>
         {
            { "cond1", true },
            { "cond2", false }
         };

         var sut = new DotNetRenderer();
         var result = sut.Render(files, tags);

         Assert.AreEqual("You chose\r\nOne\r\n", result.FirstOrDefault(x => x.Name == "file1").Content);
         Assert.AreEqual("You did not choose\r\nTwo\r\n", result.FirstOrDefault(x => x.Name == "file2").Content);
         Assert.AreEqual("You chose\r\nOne\r\nnot\r\nTwo\r\n", result.FirstOrDefault(x => x.Name == "file3").Content);
      }

      [Test]
      public void DotNetRenderer_RenderConditionalTagOnFileName()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "filecond1",
               Content = ""
            },
            new TemplateFile
            {
               Name = "filecond1/subfile",
               Content = ""
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "cond1", true }
         };

         var sut = new DotNetRenderer();
         var result = sut.Render(files, tags);

         Assert.IsTrue(result.Any(x => x.Name == "file"));
         Assert.IsTrue(result.Any(x => x.Name == "file/subfile"));
      }

      [Test]
      public void DotNetRenderer_RenderNestedConditionalTag()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "file1",
               Content = @"You chose
#if cond1
One
#if cond2
Two
#endif
#endif //cond1"
            },
            new TemplateFile
            {
               Name = "file2",
               Content = @"You chose
#if cond3
One
#if cond4
Two
#endif
#endif //cond3"
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "cond1", true },
            { "cond2", true },
            { "cond3", true },
            { "cond4", false },
         };

         var sut = new DotNetRenderer();
         var result = sut.Render(files, tags);

         Assert.AreEqual("You chose\r\nOne\r\nTwo\r\n", result.FirstOrDefault(x => x.Name == "file1").Content);
         Assert.AreEqual("You chose\r\nOne\r\n", result.FirstOrDefault(x => x.Name == "file2").Content);
      }

      [Test]
      public void DotNetRenderer_RenderMultipleConditionalTag()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "file1",
               Content = @"You chose
#if cond1
One
#endif
#if cond1
Two
#endif"
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "cond1", true }
         };

         var sut = new DotNetRenderer();
         var result = sut.Render(files, tags);

         Assert.AreEqual("You chose\r\nOne\r\nTwo\r\n", result.FirstOrDefault(x => x.Name == "file1").Content);
      }

      [Test]
      public void DotNetRenderer_RenderNameSubsetConditionalTag()
      {
         var files = new List<TemplateFile> {
            new TemplateFile
            {
               Name = "file1",
               Content = @"You chose
#if cond1
One
#endif
#if cond12
Two
#endif"
            }
         };

         var tags = new Dictionary<string, object>
         {
            { "cond1", false },
            { "cond12", true }
         };

         var sut = new DotNetRenderer();
         var result = sut.Render(files, tags);

         Assert.AreEqual("You chose\r\nTwo\r\n", result.FirstOrDefault(x => x.Name == "file1").Content);
      }
   }
}