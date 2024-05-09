using Ech.Abstractions.Database;
using Ech.Executive.Temp;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Info("Ech.Executive started");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<IRepositoryBase, TempRepository>(s =>
    {
        //var uri = s.GetRequiredService<IConfiguration>()["Database:MongoUri"];
        //Ech.Configuration.Configuration.Repository = new MongoDBRepository(uri, Ech.Configuration.Configuration.Logger);
        //return new MongoDBRepository(uri, Ech.Configuration.Configuration.Logger);
        return new TempRepository();
    });

    // NLog: setup NLog for dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

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
