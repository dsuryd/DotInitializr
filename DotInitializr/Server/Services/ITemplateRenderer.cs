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

namespace DotInitializr.Server
{
   /// <summary>
   /// Renders a set of project files from a project template and metadata.
   /// </summary>
   public interface ITemplateRenderer
   {
      /// <summary>
      /// Applies the metadata tags to a set of files from a project template.
      /// </summary>
      /// <param name="files">Project template files.</param>
      /// <param name="tags">Metadata tags.</param>
      /// <returns>Project files with the metadata applied.</returns>
      IEnumerable<TemplateFile> Render(IEnumerable<TemplateFile> files, Dictionary<string, object> tags);
   }
}