using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

#if AzureSpringCloud
using Microsoft.Azure.SpringCloud.Client;
#endif
#if ActuatorsOrDynamicLogger
using Steeltoe.Extensions.Logging.DynamicSerilog;
#endif
#if CloudFoundry
using Steeltoe.Common.Hosting;
#if !ConfigServer
using Steeltoe.Extensions.Configuration.CloudFoundry;
#endif
#if ConfigServer
using Steeltoe.Extensions.Configuration.ConfigServer;
#endif
#if PlaceholderConfig
using Steeltoe.Extensions.Configuration.Placeholder;
#endif
#if RandomValueConfig
using Steeltoe.Extensions.Configuration.RandomValue;
#endif

namespace ProjectNameSpace
{
   public class Program
   {
      public static void Main(string[] args)
      {
         CreateWebHostBuilder(args)
         .Build()
#if AnyEFCore
            .InitializeDbContexts()
#endif
            .Run();
      }

      public static IWebHostBuilder CreateWebHostBuilder(string[] args)
      {
         var builder = WebHost.CreateDefaultBuilder(args)
             .UseDefaultServiceProvider(configure => configure.ValidateScopes = false)
#if CloudFoundry
                .UseCloudHosting() //Enable listening on a Env provided port
#if !ConfigServer
                .AddCloudFoundryConfiguration() //Add cloudfoundry environment variables as a configuration source
#endif
#if ConfigServer
                .AddConfigServer()
#endif
#endif //CloudFoundry
#if PlaceholderConfig
                .AddPlaceholderResolver()
#endif
#if RandomValueConfig
                .ConfigureAppConfiguration((b) => b.AddRandomValueSource())
#endif
#if AzureSpringCloud
                .UseAzureSpringCloudService()
#endif
#if ActuatorsOrDynamicLogger
                .ConfigureLogging((context, builder) => builder.AddSerilogDynamicConsole())
#endif
                .UseStartup<Startup>();
         return builder;
      }
   }
}
