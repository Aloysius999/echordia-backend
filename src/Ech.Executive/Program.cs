using Ech.Config.Settings;
using Ech.Executive.Authentication.Middleware;
using Ech.Executive.Authentication.Services;
using Ech.Executive.Database;
using Ech.Executive.Messaging;
using Ech.Executive.Services;
using Ech.Executive.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using RabbitMQ.Client.Core.DependencyInjection;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Info("Ech.Executive started");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    //builder.Services.AddSwaggerGen();
    builder.Services.AddSwaggerGen(swagger =>
    {
        //This is to generate the Default UI of Swagger Documentation
        swagger.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "eChordia Executive API",
            Description = ".NET 8 Web API"
        });
        // To Enable authorization using Swagger (JWT)
        //swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        //{
        //    Name = "Authorization",
        //    Type = SecuritySchemeType.ApiKey,
        //    Scheme = "Bearer",
        //    BearerFormat = "JWT",
        //    In = ParameterLocation.Header,
        //    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        //});
        //swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
        //        {
        //            {
        //                  new OpenApiSecurityScheme
        //                    {
        //                        Reference = new OpenApiReference
        //                        {
        //                            Type = ReferenceType.SecurityScheme,
        //                            Id = "Bearer"
        //                        }
        //                    },
        //                    new string[] {}

        //            }
        //});
    });

    //----------------------------------------
    // configure database services
    //----------------------------------------
    var dbConfig = builder.Configuration.GetSection("DBConfiguration");

    builder.Services.Configure<DBConfiguration>(dbConfig);
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    var connectionString = dbConfig.Get<DBConfiguration>().ConnectionString;

    builder.Services.AddDbContext<MySQLDbContext>(options => options.UseMySQL(connectionString));

    //----------------------------------------
    // services
    //----------------------------------------
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IExecService, ExecService>();
    builder.Services.AddScoped<ITestService, TestService>();

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

    app.UseMiddleware<JwtMiddleware>();

    app.UseAuthorization();

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
    logger.Info("Ech.Executive terminated");

    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
