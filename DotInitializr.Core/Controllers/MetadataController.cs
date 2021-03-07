/*
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

using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace DotInitializr
{
   [Route("api/[controller]")]
   [ApiController]
   public class MetadataController : ControllerBase
   {
      [HttpPost]
      public ActionResult<TemplateMetadata> Get([FromServices] ITemplateMetadataReader templateReader, [FromBody] AppConfiguration.Template template)
      {
         TemplateMetadata metadata = null;
         try
         {
            metadata = templateReader.GetMetadata(template);
         }
         catch (TemplateException ex)
         {
            Response.StatusCode = (int) HttpStatusCode.BadRequest;
            return Content(ex.Message);
         }

         metadata ??= new TemplateMetadata();
         if (template != null)
         {
            var tags = metadata.Tags?.ToList() ?? new List<Tag>();

            if (!tags.Any(x => x.Name == TemplateMetadataReader.PROJECT_NAME || x.Key == TemplateMetadataReader.PROJECT_NAME_KEY))
               tags.Insert(0, new Tag
               {
                  Key = TemplateMetadataReader.PROJECT_NAME_KEY,
                  Name = TemplateMetadataReader.PROJECT_NAME,
                  DefaultValue = TemplateMetadataReader.DEFAULT_PROJECT_NAME,
                  ValidationRegex = @"^[\w\-. ]+$",
                  ValidationError = "Must be a valid filename",
               });

            metadata.Tags = tags;
         }

         return metadata;
      }
   }
}