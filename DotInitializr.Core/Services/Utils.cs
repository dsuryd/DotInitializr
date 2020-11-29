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

namespace DotInitializr
{
   public class Utils
   {
      public static void DeleteDirectory(string path)
      {
         if (!Directory.Exists(path))
            return;

         try
         {
            foreach (var fileName in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
               var fileInfo = new FileInfo(fileName);
               fileInfo.Attributes = FileAttributes.Normal;
               fileInfo.Delete();
            }

            Directory.Delete(path, true);
         }
         catch (Exception ex)
         {
            Console.WriteLine($"Failed to delete `{path}`: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
         }
      }
   }
}