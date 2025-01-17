using Azure.Communication.Email;
using EmailProvider.Contexts;
using EmailProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<DataContext>(x => x.UseCosmos(Environment.GetEnvironmentVariable("Cosmos_Db_ConnectionString"), Environment.GetEnvironmentVariable("Cosmos_Db_Name")));
        services.AddSingleton(new EmailClient(Environment.GetEnvironmentVariable("Communication_Services")));
        services.AddSingleton<EmailService>();
    })
    .Build();

host.Run();
