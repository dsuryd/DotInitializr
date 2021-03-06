using System.Diagnostics;
using DotNetify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotInitializr.Website.Server
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
         services.AddDotInitializr(Configuration);

         services.AddTransient<MetadataForm>();
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

         // Don't use DefaultTraceListener to prevent "fail fast".
         Trace.Listeners.Clear();
         Trace.Listeners.Add(new ConsoleTraceListener());

         app.UseHttpsRedirection();

         app.UseWebSockets();
         app.UseDotNetify(config =>
         {
            if (env.IsDevelopment())
               config.UseDeveloperLogging();
         });

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