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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DotInitializr
{
   public static class Extensions
   {
      public static IServiceCollection AddDotInitializr(this IServiceCollection services) => services.AddDotInitializr(null, false);

      public static IServiceCollection AddDotInitializr(this IServiceCollection services, IConfiguration configuration, bool includeControllers = true)
      {
         services.AddSingleton<ITemplateSource, GitTemplateSource>();
         services.AddSingleton<ITemplateRenderer, MustacheRenderer>();
         services.AddSingleton<ITemplateRenderer, DotNetRenderer>();
         services.AddSingleton<IProjectGenerator, ProjectGenerator>();
         services.AddSingleton<ITemplateMetadataReader, TemplateMetadataReader>();

         if (includeControllers)
            services.AddMvcCore().AddApplicationPart(typeof(Extensions).Assembly).AddControllersAsServices();
            
         if (configuration != null)
         {
             services.AddSingleton(configuration.GetSection(AppConfiguration.SECTION).Get<AppConfiguration>() ?? new AppConfiguration());
             services.AddSingleton(configuration.GetSection(PersonalAccessTokenAuthenticationOptions.SECTION).Get<PersonalAccessTokenAuthenticationOptions>() ?? new PersonalAccessTokenAuthenticationOptions());
         }

			return services;
      }

      public static string ToJson(this DotNetTemplateMetadata self) => JsonConvert.SerializeObject(self, DotInitializr.Converter.Settings);

      public static DotNetTemplateMetadata ToDotNetTemplateMetadata(this string json) => JsonConvert.DeserializeObject<DotNetTemplateMetadata>(json, DotInitializr.Converter.Settings);
   }
}