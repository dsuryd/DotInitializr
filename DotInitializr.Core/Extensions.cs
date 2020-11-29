using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotInitializr
{
   public static class Extensions
   {
      public static IServiceCollection AddDotInitializr(this IServiceCollection services, IConfiguration configuration)
      {
         services.AddSingleton<ITemplateSource, GitTemplateSource>();
         services.AddSingleton<ITemplateRenderer, MustacheRenderer>();
         services.AddSingleton<ITemplateRenderer, DotNetRenderer>();
         services.AddSingleton<IProjectGenerator, ProjectGenerator>();
         services.AddSingleton<ITemplateMetadataReader, TemplateMetadataReader>();
         services.AddMvcCore().AddApplicationPart(typeof(Extensions).Assembly).AddControllersAsServices();
         services.AddSingleton(configuration.GetSection(AppConfiguration.SECTION).Get<AppConfiguration>() ?? new AppConfiguration());

         return services;
      }
   }
}