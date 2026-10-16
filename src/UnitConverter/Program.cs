using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application host...");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    string logPath = Path.Combine(
        builder.Environment.ContentRootPath,
        "Logs",
        "unitconverter-.json");

    builder.Host.UseSerilog((context, configuration) => configuration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(
            new RenderedCompactJsonFormatter(),
            logPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            shared: true));

    builder.Services.AddRazorPages();

    builder.Services.AddSingleton<UnitConverter.Services.IConversionService, UnitConverter.Services.UnitOfConversionService>();
    builder.Services.AddSingleton<UnitConverter.Services.ILogReader, UnitConverter.Services.JsonLogReader>();

    WebApplication app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();
    app.UseStaticFiles();
    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly during lifecycle processing");
}
finally
{
    Log.CloseAndFlush();
}

namespace UnitConverter
{
    /// <summary>
    ///     This is provided to support the WebApplicationFactory used in testing.
    /// </summary>
    public class Program
    {
    }
}
