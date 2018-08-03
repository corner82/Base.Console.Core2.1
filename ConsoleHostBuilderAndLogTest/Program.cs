using System;
using System.IO;
using System.Threading.Tasks;
using ConsoleHostBuilderAndLogTest.ConfigModels;
using ConsoleHostBuilderAndLogTest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace ConsoleHostBuilderAndLogTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


            Log.Logger = CreateloggerConf();

            try
            {
                Log.Information("Starting host");
                var host = BuildHost(args);

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        /*var dbContext = scope.ServiceProvider.GetService<PageAccessLogContext>();
                        var app = scope.ServiceProvider.GetService<App>();
                        app.Run();*/
                        var db = services.GetService<PageAccessLogContext>();
                        ////var db = services.GetRequiredService<PageAccessLogContext>();
                        db.Database.Migrate();
                        //SeeData is a class in my Models folder
                        ////PageAccessLog.Initialize(services);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }

                await host.RunAsync();
                //return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                //return 1;
                
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }


        public static IHost BuildHost(string[] args) =>
             new HostBuilder()
                //.UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureHostConfiguration(configHost =>
                {
                    //configHost.SetBasePath(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\")));
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    //configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddEnvironmentVariables();
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    //configApp.SetBasePath(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\")));
                    configApp.AddJsonFile(
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    //configApp.AddEnvironmentVariables(prefix: "PREFIX_");

                    if (args != null)
                    {
                        configApp.AddCommandLine(args);   
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddOptions();
                    //services.Configure<ConfigRabbitMQ>(configHost.Configuration.GetSection("RabbitMQLog"));
                    //services.Configure<ConfigRabbitMQ>(hostContext.Configuration.GetSection("RabbitMQLog"));
                    services.Configure<ConfigRabbitMQ>(hostContext.Configuration.GetSection("RabbitMQLog"));
                    services.AddLogging();
                    //services.AddHostedService<LifetimeEventsHostedService>();
                    services.AddHostedService<TimedHostedService>();
                    services.AddHostedService<RabbitMQSubscriberService>();

                    //Identity dccontext ayarları(postgreSQL için ayarlanıyor veya sql server için )
                    services.AddDbContext<PageAccessLogContext>(options =>
                                //options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
                                options.UseSqlServer("Server = localhost\\SQLEXPRESS; Database = SIS; user id = sa; password = 1q2w3e4r; MultipleActiveResultSets = true"

                                    //options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")
                                    //, b => b.MigrationsAssembly("Miya.Core.Entities.Identity")

                                    )
                    );


                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                    //configLogging.AddSerilog();

                })
                    .UseConsoleLifetime()
                    .UseSerilog()
                    .Build();

        public static Serilog.ILogger CreateloggerConf()
        {
            var conf = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           .Enrich.FromLogContext()
           //.WriteTo.Console()
           .WriteTo.File("Logs\\RabbitMQPageAccessLog-.log",
                            fileSizeLimitBytes: null,
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 150)
           .CreateLogger();
            return conf;
        }
    }
}
