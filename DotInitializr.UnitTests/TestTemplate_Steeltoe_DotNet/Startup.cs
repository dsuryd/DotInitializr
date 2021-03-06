using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#if MongoDB
using Steeltoe.Connector.MongoDb;
#endif
#if MySqlOrMySqlEFCore
using Steeltoe.Connector.MySql;
#endif
#if MySqlEFCore
using Steeltoe.Connector.MySql.EFCore;
#endif
#if OAuthConnector
using Steeltoe.Connector.OAuth;
#endif
#if Postgres
using Steeltoe.Connector.PostgreSql;
#endif
#if PostgresEFCore
using Steeltoe.Connector.PostgreSql.EFCore;
#endif
#if RabbitMQ
using Steeltoe.Connector.RabbitMQ;
#endif
#if Redis
using Steeltoe.Connector.Redis;
#endif
#if SQLServer
using Steeltoe.Connector.SqlServer.EFCore;
#endif
#if Discovery
using Steeltoe.Discovery.Client;
#endif
#if Actuators
using Steeltoe.Management.CloudFoundry;
#endif
#if RequiresHttps
using Microsoft.AspNetCore.HttpsPolicy;
#endif
#if Auth
using Microsoft.AspNetCore.Authentication;
#endif
#if OrganizationalAuth
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
#endif
#if IndividualB2CAuth
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
#endif
#if CircuitBreaker
using Steeltoe.CircuitBreaker.Hystrix;
#endif

namespace ProjectNameSpace
{
   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
#if OrganizationalAuth
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));
#endif
#if IndividualB2CAuth
            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
#endif
#if MySql
            services.AddMySqlConnection(Configuration);
#endif
#if Actuators
            services.AddCloudFoundryActuators(Configuration);
#endif
#if Discovery
            services.AddDiscoveryClient(Configuration);
#endif
#if Postgres
            services.AddPostgresConnection(Configuration);
#endif
#if RabbitMQ
            services.AddRabbitMQConnection(Configuration);
#endif
#if Redis
            // Add the Redis distributed cache.
            // We are using the Steeltoe Redis Connector to pickup the CloudFoundry
            // Redis Service binding and use it to configure the underlying RedisCache
            // This adds a IDistributedCache to the container
            services.AddDistributedRedisCache(Configuration);
            // This works like the above, but adds a IConnectionMultiplexer to the container
            // services.AddRedisConnectionMultiplexer(Configuration);
#endif
#if MongoDB
            services.AddMongoClient(Configuration);
#endif
#if OAuthConnector
            services.AddOAuthServiceOptions(Configuration);
#endif
#if PostgresEFCore
            // Add Context and use Postgres as provider ... provider will be configured from VCAP_ info
            // services.AddDbContext<MyDbContext>(options => options.UseNpgsql(Configuration));
#endif
         services.AddControllers();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }
#if RequiresHttps
            else
            {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
#endif
#if Auth
            app.UseAuthentication();
#endif

#if Discovery
            app.UseDiscoveryClient();
#endif
         app.UseRouting();
         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllers();
         });
      }
   }
}