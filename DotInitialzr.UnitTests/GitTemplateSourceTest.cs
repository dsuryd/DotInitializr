using System.Linq;
using System.Text.Json;
using DotInitialzr.Server;
using NUnit.Framework;

namespace DotInitialzr.UnitTests
{
   public class GitTemplateSourceTest
   {
      [Test]
      [TestCase("https://github.com/dsuryd/DotInitialzr", "DotInitialzr.UnitTests\\TestTemplate")]
      public void GitTemplateSource_GetFiles_ReturnsFiles(string sourceUrl, string directory)
      {
         var source = new GitTemplateSource();
         var files = source.GetFiles(sourceUrl, directory);

         Assert.Greater(files.Count(), 0);
      }

      [Test]
      [TestCase("https://github.com/dsuryd/DotInitialzr", "DotInitialzr.UnitTests\\TestTemplate", "dotInitialzr.json")]
      public void GitTemplateSource_GetFile_ReturnsFile(string sourceUrl, string directory, string fileName)
      {
         var source = new GitTemplateSource();
         var file = source.GetFile(fileName, sourceUrl, directory);

         Assert.IsNotNull(file);
         Assert.IsFalse(string.IsNullOrEmpty(file.Name));
         Assert.Greater(file.Content?.Length, 0);
      }
   }
}