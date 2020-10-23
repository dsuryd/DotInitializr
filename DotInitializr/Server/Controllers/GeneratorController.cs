using System;
using System.Net;
using System.Linq;
using DotInitializr.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DotInitializr.Server
{
   [Route("api/[controller]")]
   [ApiController]
   public class GeneratorController : ControllerBase
   {
      [HttpPost]
      public ActionResult Get([FromServices] IProjectGenerator generator, [FromBody] ProjectMetadata metadata)
      {
         if (metadata.Tags != null)
         {
            foreach (var kvp in metadata.Tags.Where(x => x.Value is JsonElement).ToList())
            {
               var jsonElem = (JsonElement) kvp.Value;
               if (jsonElem.ValueKind == JsonValueKind.True)
                  metadata.Tags[kvp.Key] = true;
               else if (jsonElem.ValueKind == JsonValueKind.False)
                  metadata.Tags[kvp.Key] = false;
               else
                  metadata.Tags[kvp.Key] = jsonElem.GetString();
            }
         }

         try
         {
            byte[] templateBytes = generator.Generate(metadata);
            return new FileContentResult(templateBytes, "application/zip") { FileDownloadName = $"{metadata.ProjectName}.zip" };
         }
         catch (TemplateException ex)
         {
            Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            return Content(ex.Message);
         }
         catch (Exception ex)
         {
            Response.StatusCode = (int) HttpStatusCode.BadRequest;
            return Content(ex.Message);
         }
      }
   }
}