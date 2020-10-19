using System.Collections.Generic;
using System.Linq;
using DotInitializr.Server;
using NUnit.Framework;

namespace DotInitializr.UnitTests
{
   public class TemplateReaderTest
   {
      [Test]
      [TestCase("https://github.com/dsuryd/DotInitializr", "DotInitializr.UnitTests\\TestTemplate")]
      public void TemplateReader_GetMetadata_ReturnsMetadata(string sourceUrl, string directory)
      {
         var sut = new TemplateReader(new List<ITemplateSource> { new GitTemplateSource() });

         var result = sut.GetMetadata(new AppConfiguration.Template { Name = "TestTemplate", SourceType = "git", SourceUrl = sourceUrl, SourceDirectory = directory });

         Assert.Greater(result.TextTags?.Count(), 0);
         Assert.Greater(result.ConditionalTags?.Count(), 0);
      }

      [Test]
      public void TemplateReader_GetMetadataTags_ReturnsTextAndConditionalTagsWithDefaultValues()
      {
         var sut = new TemplateReader(new List<ITemplateSource> { new GitTemplateSource() });

         var metadata = new TemplateMetadata
         {
            TextTags = new List<TextTemplateTag>
            {
               new TextTemplateTag { Key = "text1", DefaultValue = "abc"},
               new TextTemplateTag { Key = "text2", DefaultValue = "xyz"}
            },
            ConditionalTags = new List<ConditionalTemplateTag>
            {
               new ConditionalTemplateTag { Key = "cond1", DefaultValue = true },
               new ConditionalTemplateTag { Key = "cond2", DefaultValue = false }
            },
            ComputedTags = new List<ComputedTemplateTag>
            {
               new ComputedTemplateTag { Key = "computed1", Expression = "cond1 || cond2"}
            }
         };

         var result = sut.GetMetadataTags(metadata);

         Assert.AreEqual(4, result.Count);
         Assert.AreEqual("abc", result["text1"]);
         Assert.AreEqual("xyz", result["text2"]);
         Assert.AreEqual(true, result["cond1"]);
         Assert.AreEqual(false, result["cond2"]);
      }

      [Test]
      public void TemplateReader_GetFilesToExclude_ReturnsFilesToExcludeString()
      {
         var sut = new TemplateReader(new List<ITemplateSource> { new GitTemplateSource() });

         var metadata = new TemplateMetadata
         {
            ConditionalTags = new List<ConditionalTemplateTag>
            {
               new ConditionalTemplateTag { Key = "cond1", DefaultValue = true, FilesToInclude ="cond1_file" },
               new ConditionalTemplateTag { Key = "cond2", DefaultValue = false, FilesToInclude = "cond2_file1,cond2_file2" }
            }
         };

         var result = sut.GetFilesToExclude(metadata, new Dictionary<string, bool> { { "cond1", true } });
         Assert.AreEqual("cond2_file1,cond2_file2", result);

         result = sut.GetFilesToExclude(metadata, new Dictionary<string, bool> { { "cond1", false }, { "cond2", true } });
         Assert.AreEqual("cond1_file", result);

         result = sut.GetFilesToExclude(metadata, new Dictionary<string, bool> { { "cond1", false }, { "cond2", false } });
         Assert.AreEqual("cond1_file,cond2_file1,cond2_file2", result);
      }

      [Test]
      public void TemplateReader_GetComputedTags_ReturnsComputedTags()
      {
         var sut = new TemplateReader(new List<ITemplateSource> { new GitTemplateSource() });

         var metadata = new TemplateMetadata
         {
            ConditionalTags = new List<ConditionalTemplateTag>
            {
               new ConditionalTemplateTag { Key = "cond1", DefaultValue = true  },
               new ConditionalTemplateTag { Key = "cond2", DefaultValue = false  }
            },
            ComputedTags = new List<ComputedTemplateTag>
            {
               new ComputedTemplateTag { Key = "computed1", Expression = "cond1 || cond2" },
               new ComputedTemplateTag { Key = "computed2", Expression = "cond1 && cond2" }
            }
         };

         var result1 = sut.GetComputedTags(metadata, new Dictionary<string, bool> { { "cond1", false }, { "cond2", true } });
         var result2 = sut.GetComputedTags(metadata, new Dictionary<string, bool> { { "cond1", true }, { "cond2", true } });

         Assert.AreEqual(1, result1.Count);
         Assert.AreEqual(2, result2.Count);
      }
   }
}