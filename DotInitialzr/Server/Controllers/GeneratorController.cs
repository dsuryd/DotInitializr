using System;
using System.Net;
using System.Net.Http.Headers;
using DotInitialzr.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DotInitialzr.Server
{
   [Route("api/[controller]")]
   [ApiController]
   public class GeneratorController : ControllerBase
   {
      [HttpPost]
      public ActionResult Get([FromServices] IProjectGenerator generator, [FromBody] TemplateMetadata metadata)
      {
         try
         {
            byte[] templateBytes = generator.Generate(metadata);
            return new FileContentResult(templateBytes, "application/zip") { FileDownloadName = $"{metadata.ProjectName}.zip" };
         }
         catch (Exception ex)
         {
            Response.StatusCode = (int) HttpStatusCode.BadRequest;
            return Content(ex.Message);
         }
      }
   }
}