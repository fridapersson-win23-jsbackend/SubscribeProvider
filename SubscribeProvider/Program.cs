using Data.Contexts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
.ConfigureServices((context, services) =>
{
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();

    services.AddDbContext<DataContext>(x => x.UseSqlServer(context.Configuration.GetConnectionString("SqlServer")));

    //var connectionString = context.Configuration.GetConnectionString("SqlServer");

    //var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
    //var logger = loggerFactory.CreateLogger<Program>();
    //logger.LogInformation($"Using connection string: {connectionString}");

    //services.AddDbContext<DataContext>(options =>
    //    options.UseSqlServer(connectionString));
})
    .Build();

host.Run();
