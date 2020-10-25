using System.Collections.Generic;
using System.Linq;
using DotInitializr.Server;
using NUnit.Framework;

namespace DotInitializr.UnitTests
{
   public class TemplateMetadataReaderTest
   {
      [Test]
      [TestCase("https://github.com/dsuryd/DotInitializr", "DotInitializr.UnitTests\\TestTemplate")]
      public void TemplateMetadataReader_GetMetadata_ReturnsMetadata(string sourceUrl, string directory)
      {
         var sut = new TemplateMetadataReader(new List<ITemplateSource> { new GitTemplateSource() });

         var result = sut.GetMetadata(new AppConfiguration.Template { Name = "TestTemplate", SourceType = "git", SourceUrl = sourceUrl, SourceDirectory = directory });

         Assert.Greater(result.Tags?.Count(), 0);
         Assert.Greater(result.ComputedTags?.Count(), 0);
      }

      [Test]
      public void TemplateMetadataReader_GeTags_ReturnsTagsWithDefaultValues()
      {
         var sut = new TemplateMetadataReader(new List<ITemplateSource> { new GitTemplateSource() });

         var metadata = new TemplateMetadata
         {
            Tags = new List<Tag>
            {
               new Tag { Key = "text1", DefaultValue = "abc" },
               new Tag { Key = "text2", DefaultValue = "xyz" },
               new Tag { Key = "dropdown", DefaultValue = "option1", Options = new string[] { "option1", "option2" } }
            },
            ConditionalTags = new List<ConditionalTag>
            {
               new ConditionalTag { Key = "cond1", DefaultValue = true },
               new ConditionalTag { Key = "cond2", DefaultValue = false }
            },
            ComputedTags = new List<ComputedTag>
            {
               new ComputedTag { Key = "computed1", Expression = "cond1 || cond2"}
            }
         };

         var result = sut.GetTags(metadata);

         Assert.AreEqual(3, result.Count);
         Assert.AreEqual("abc", result["text1"]);
         Assert.AreEqual("xyz", result["text2"]);
         Assert.AreEqual("option1", result["dropdown"]);
      }

      [Test]
      public void TemplateMetadataReader_GeConditionalTags_ReturnsConditionalTagsWithDefaultValues()
      {
         var sut = new TemplateMetadataReader(new List<ITemplateSource> { new GitTemplateSource() });

         var metadata = new TemplateMetadata
         {
            Tags = new List<Tag>
            {
               new Tag { Key = "text1", DefaultValue = "abc" },
               new Tag { Key = "text2", DefaultValue = "xyz" },
               new Tag { Key = "dropdown", DefaultValue = "option1", Options = new string[] { "option1", "option2" } }
            },
            ConditionalTags = new List<ConditionalTag>
            {
               new ConditionalTag { Key = "cond1", DefaultValue = true },
               new ConditionalTag { Key = "cond2", DefaultValue = false }
            },
            ComputedTags = new List<ComputedTag>
            {
               new ComputedTag { Key = "computed1", Expression = "cond1 || cond2"}
            }
         };

         var result = sut.GetConditionalTags(metadata);

         Assert.AreEqual(2, result.Count);
         Assert.AreEqual(true, result["cond1"]);
         Assert.AreEqual(false, result["cond2"]);
      }

      [Test]
      public void TemplateMetadataReader_GetFilesToExclude_ReturnsFilesToExcludeString()
      {
         var sut = new TemplateMetadataReader(new List<ITemplateSource> { new GitTemplateSource() });

         var metadata = new TemplateMetadata
         {
            ConditionalTags = new List<ConditionalTag>
            {
               new ConditionalTag { Key = "cond1", DefaultValue = true, FilesToInclude ="cond1_file" },
               new ConditionalTag { Key = "cond2", DefaultValue = false, FilesToInclude = "cond2_file1,cond2_file2" }
            },
            ComputedTags = new List<ComputedTag>
            {
               new ComputedTag { Key = "computed1", Expression = "!cond1 && !cond2", FilesToInclude = "computed1_file" }
            }
         };

         var result = sut.GetFilesToExclude(metadata, new Dictionary<string, bool> { { "cond1", true } });
         Assert.AreEqual("cond2_file1,cond2_file2,computed1_file", result);

         result = sut.GetFilesToExclude(metadata, new Dictionary<string, bool> { { "cond1", false }, { "cond2", true } });
         Assert.AreEqual("cond1_file,computed1_file", result);

         result = sut.GetFilesToExclude(metadata, new Dictionary<string, bool> { { "cond1", false }, { "cond2", false }, { "computed1", true } });
         Assert.AreEqual("cond1_file,cond2_file1,cond2_file2", result);
      }

      [Test]
      public void TemplateMetadataReader_GetComputedTags_ReturnsValidTags()
      {
         var sut = new TemplateMetadataReader(new List<ITemplateSource> { new GitTemplateSource() });

         var metadata = new TemplateMetadata
         {
            ConditionalTags = new List<ConditionalTag>
            {
               new ConditionalTag { Key = "cond1", DefaultValue = true  },
               new ConditionalTag { Key = "cond2", DefaultValue = false  }
            },
            ComputedTags = new List<ComputedTag>
            {
               new ComputedTag { Key = "computed1", Expression = "cond1 || cond2" },
               new ComputedTag { Key = "computed2", Expression = "cond1 && cond2" }
            }
         };

         var result1 = sut.GetComputedTags(metadata, new Dictionary<string, object> { { "cond1", false }, { "cond2", true } });
         var result2 = sut.GetComputedTags(metadata, new Dictionary<string, object> { { "cond1", true }, { "cond2", true } });

         Assert.AreEqual(1, result1.Count);
         Assert.AreEqual(2, result2.Count);
      }

      [Test]
      public void TemplateMetadataReader_GetComputedTags_CountExpression_ReturnsValidTags()
      {
         var sut = new TemplateMetadataReader(new List<ITemplateSource> { new GitTemplateSource() });

         var metadata = new TemplateMetadata
         {
            ConditionalTags = new List<ConditionalTag>
            {
               new ConditionalTag { Key = "cond1", DefaultValue = true  },
               new ConditionalTag { Key = "cond2", DefaultValue = false  }
            },
            ComputedTags = new List<ComputedTag>
            {
               new ComputedTag { Key = "computed1", Expression = "Count(cond1,cond2) > 1" }
            }
         };

         var result1 = sut.GetComputedTags(metadata, new Dictionary<string, object> { { "cond1", false }, { "cond2", true } });
         var result2 = sut.GetComputedTags(metadata, new Dictionary<string, object> { { "cond1", true }, { "cond2", true } });

         Assert.IsFalse(result1.ContainsKey("computed1"));
         Assert.IsTrue(result2.ContainsKey("computed1"));
      }

      [Test]
      public void TemplateMetadataReader_GetComputedTagsEqual_EqualExpression_ReturnsValidTag()
      {
         var sut = new TemplateMetadataReader(new List<ITemplateSource> { new GitTemplateSource() });

         var metadata = new TemplateMetadata
         {
            Tags = new List<Tag>
            {
               new Tag { Key = "tag1", Options = new string[] { "A", "B" } }
            },
            ComputedTags = new List<ComputedTag>
            {
               new ComputedTag { Key = "computed1", Expression = "tag1 == \"B\"" }
            }
         };

         var result1 = sut.GetComputedTags(metadata, new Dictionary<string, object> { { "tag1", "A" } });
         var result2 = sut.GetComputedTags(metadata, new Dictionary<string, object> { { "tag1", "B" } });

         Assert.IsFalse(result1.ContainsKey("computed1"));
         Assert.IsTrue(result2.ContainsKey("computed1"));
      }
   }
}