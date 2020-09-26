using System.IO;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DotInitialzr.Server;
using DotInitialzr.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace DotInitialzr.UnitTests
{
   internal class GeneratorControllerTest
   {
      [Test]
      public async Task GeneratorController_Get_ReturnsZippedProject()
      {
         var client = new WebApplicationFactory<Startup>().CreateClient();

         var metadata = new TemplateMetadata
         {
            ProjectName = "MyStarter",
            TemplateSourceType = "git",
            TemplateSourceUrl = "https://github.com/dsuryd/dotNetify-react-template",
            TemplateSourceDirectory = "ReactTemplate\\content"
         };
         var response = await client.PostAsJsonAsync("/api/generator", metadata);

         var fileName = response.Content.Headers.ContentDisposition.FileName;
         var tempPath = Path.Combine(Path.GetTempPath(), nameof(DotInitialzr), fileName);
         using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
         {
            await response.Content.CopyToAsync(fileStream);
         }

         Utils.DeleteDirectory(tempPath);
      }
   }
}