using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using D3.AccountLedger.Api.Grains;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace D3.AccountLedger.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseOrleans((context, siloBuilder) =>
                {

                    siloBuilder.ConfigureApplicationParts(parts =>
                    {
                        parts.AddApplicationPart(typeof(AccountLedgerGrain).Assembly).WithReferences();
                    });
                    
                    siloBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "my-cluster-id";
                        options.ServiceId = "my-service-id";
                    });

                    siloBuilder.ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000);

                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        siloBuilder.ConfigureLogging(logging => logging.AddConsole());
                        siloBuilder.UseLocalhostClustering();
                        siloBuilder.AddMemoryGrainStorageAsDefault();
                        siloBuilder.UseInMemoryReminderService();

                    }
                    else
                    {
                        var azureStorageConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE") ??
                                                           context.Configuration.GetConnectionString("AzureStorage");

                        siloBuilder.UseKubernetesHosting();
                        siloBuilder.UseAzureStorageClustering(options =>
                            options.ConnectionString = azureStorageConnectionString);
                        siloBuilder.AddAzureTableGrainStorageAsDefault(az =>
                        {
                            az.UseJson = true;
                            az.ConnectionString = azureStorageConnectionString;
                        });
                        siloBuilder.UseAzureTableReminderService(options =>
                        {
                            options.ConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE") ??
                                                       context.Configuration.GetConnectionString("AzureStorage");
                        });
                    }
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
