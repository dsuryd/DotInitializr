using System.Linq;
using DotInitialzr.Server;
using NUnit.Framework;

namespace DotInitialzr.UnitTests
{
   public class GitTemplateSourceTest
   {
      [Test]
      [TestCase("https://github.com/dsuryd/dotNetify-react-template", "ReactTemplate\\content")]
      public void GitTemplateSource_GetFiles_ReturnsFiles(string sourceUrl, string directory)
      {
         var source = new GitTemplateSource();
         var files = source.GetFiles(sourceUrl, directory);

         Assert.Greater(files.Count(), 0);
      }
   }
}