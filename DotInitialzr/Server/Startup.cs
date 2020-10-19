using DotNetify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotInitialzr.Server
{
   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      public void ConfigureServices(IServiceCollection services)
      {
         services.AddSignalR();
         services.AddDotNetify();
         services.AddMvc();

         services.AddSingleton<ITemplateSource, GitTemplateSource>();
         services.AddSingleton<ITemplateRenderer, MustacheRenderer>();
         services.AddSingleton<IProjectGenerator, ProjectGenerator>();
         services.AddSingleton<ITemplateReader, TemplateReader>();
         services.AddTransient<MetadataForm>();

         services.AddSingleton(Configuration.GetSection(AppConfiguration.SECTION).Get<AppConfiguration>() ?? new AppConfiguration());
      }

      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
         }
         else
         {
            app.UseExceptionHandler("/Error");
         }

         app.UseWebSockets();
         app.UseDotNetify(config => config.UseDeveloperLogging());

         app.UseBlazorFrameworkFiles();
         app.UseStaticFiles();
         app.UseRouting();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllers();
            endpoints.MapHub<DotNetifyHub>("/dotnetify");
            endpoints.MapFallbackToFile("index.html");
         });
      }
   }
}