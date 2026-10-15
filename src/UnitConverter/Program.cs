WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSingleton<UnitConverter.Services.IConversionService, UnitConverter.Services.UnitOfConversionService>();


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


namespace UnitConverter
{
    /// <summary>
    ///     This is provided to support the WebApplicationFactory used in testing.
    /// </summary>
    public class Program
    {
    }
}
