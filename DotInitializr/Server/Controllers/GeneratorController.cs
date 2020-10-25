﻿/*
Copyright 2020 Dicky Suryadi

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

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