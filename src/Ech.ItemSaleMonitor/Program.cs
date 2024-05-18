using Ech.Config.Settings;
using Ech.ItemSaleMonitor.Database;
using Ech.ItemSaleMonitor.Messaging;
using Ech.ItemSaleMonitor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using RabbitMQ.Client.Core.DependencyInjection;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    //----------------------------------------
    // Add services to the container.
    //----------------------------------------

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(swagger =>
    {
        //This is to generate the Default UI of Swagger Documentation
        swagger.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "eChordia Item Sale Monitor",
            Description = ".NET 8 Service"
        });
    });

    //----------------------------------------
    // configure database services
    //----------------------------------------
    var dbConfig = builder.Configuration.GetSection("DBConfiguration");
    builder.Services.Configure<DBConfiguration>(dbConfig);

    var connectionString = dbConfig.Get<DBConfiguration>().ConnectionString;

    builder.Services.AddDbContext<MySQLDbContext>(options => options.UseMySQL(connectionString));

    //----------------------------------------
    // services
    //----------------------------------------
    builder.Services.AddScoped<IMessagingService, MessagingService>();

    //----------------------------------------
    // configure RabbitMQ
    //----------------------------------------
    var rabbitMqSection = builder.Configuration.GetSection("RabbitMq");
    var exchangeSection = builder.Configuration.GetSection("RabbitMqExchange");

    // consumer
    var myRabbitMqSection = builder.Configuration.GetSection("MyRabbitMq");
    var routeKey = myRabbitMqSection.GetValue<string>("RoutingKeyReceive");

    builder.Services
        .AddRabbitMqServices(rabbitMqSection)
        .AddConsumptionExchange("ech.exchange", exchangeSection)
        .AddMessageHandlerSingleton<CustomMessageHandler>(routeKey);

    // producer
    builder.Services.AddRabbitMqProducer(rabbitMqSection)
        .AddProductionExchange("ech.exchange", exchangeSection);

    //----------------------------------------
    // NLog: setup NLog for dependency injection
    //----------------------------------------
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Host.UseNLog();

    var app = builder.Build();

    //----------------------------------------
    // Configure the HTTP request pipeline.
    //----------------------------------------
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    //app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    //logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    logger.Info("Ech.ItemSaleMonitor terminated");

    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}