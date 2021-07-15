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

        [Test]
        [TestCase("testTemplate.json")]
        public void DotNetTemplateMetadataMapper_MapGeneratedSymbols(string templateJsonFile)
        {
            var content = File.ReadAllText(templateJsonFile);
            var templateMetadata = DotNetTemplateMetadata.FromJson(content);
            var result = new DotNetTemplateMetadataMapper().Map(templateMetadata);

            Assert.IsNotNull(result);

            var httpPortConstant = result.ComputedTags.FirstOrDefault(x => x.Key == "HttpPortConstant");
            Assert.AreEqual("5000", httpPortConstant.Expression);

            var httpPortReplacer = result.ComputedTags.FirstOrDefault(x => x.Key == "5000");
            Assert.AreEqual("HttpPort != null ? HttpPort : HttpPortConstant", httpPortReplacer.Expression);

            var gitHubProjectLower = result.ComputedTags.FirstOrDefault(x => x.Key == "GitHubProjectLower");
            Assert.AreEqual("lowerCase(GitHubProject)", gitHubProjectLower.Expression);
        }
    }
}