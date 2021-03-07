using System.IO;
using System.Linq;
using NUnit.Framework;

namespace DotInitializr.UnitTests
{
   public class DotNetTemplateMetadataMapperTest
   {
      [Test]
      [TestCase("testTemplate.json")]
      public void DotNetTemplateMetadataMapper_Map(string templateJsonFile)
      {
         var content = File.ReadAllText(templateJsonFile);
         var templateMetadata = DotNetTemplateMetadata.FromJson(content);
         var result = new DotNetTemplateMetadataMapper().Map(templateMetadata);

         Assert.IsNotNull(result);
         Assert.IsTrue(result.Tags.Count() > 0);
         Assert.IsTrue(result.ConditionalTags.Count() > 0);
         Assert.IsTrue(result.ComputedTags.Count() > 0);
      }
   }
}