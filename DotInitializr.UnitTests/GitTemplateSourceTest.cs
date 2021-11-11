using System.Linq;
using NUnit.Framework;

namespace DotInitializr.UnitTests
{
   public class GitTemplateSourceTest
   {
      [Test]
      [TestCase("https://github.com/dsuryd/DotInitializr", "DotInitializr.UnitTests\\TestTemplate")]
      public void GitTemplateSource_GetFiles_ReturnsFiles(string sourceUrl, string directory, string branch = null)
      {
         var source = new GitTemplateSource();
         var files = source.GetFiles(sourceUrl, directory, branch);

         Assert.AreEqual(74, files.Count());

         Assert.AreEqual(3, files.Where(x => x is TemplateFileBinary).Count());
      }

      [Test]
      [TestCase("https://github.com/dsuryd/DotInitializr", "DotInitializr.UnitTests\\TestTemplate", "dotInitializr.json")]
      public void GitTemplateSource_GetFile_ReturnsFile(string sourceUrl, string directory, string fileName, string branch = null)
      {
         var source = new GitTemplateSource();
         var file = source.GetFile(fileName, sourceUrl, directory, branch);

         Assert.IsNotNull(file);
         Assert.IsFalse(string.IsNullOrEmpty(file.Name));
         Assert.Greater(file.Content?.Length, 0);
      }
   }
}