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

using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace DotInitializr
{
   [Route("api/[controller]")]
   [ApiController]
   public class GitController : ControllerBase
   {
      [HttpGet]
      public ActionResult<string> Get([FromServices] ITemplateSource templateSource, [FromQuery] string source, [FromQuery] string path)
      {
         try
         {
            string fileName = Path.GetFileName(path);
            string directory = Path.GetDirectoryName(path);
            var templateFile = templateSource.GetFile(fileName, source, directory);
            return templateFile.Content;
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