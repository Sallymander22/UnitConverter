using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UnitConverter.Tests;

/// <summary>
///     Acceptance tests for Lesson 1: First Razor Page.
///     These tests use reflection where appropriate so that the test project
///     still compiles before the student creates ConversionsModel.
/// </summary>
public class Lesson01Tests
{
    private const string RequiredInput = "3.1415";
    private const string RequiredConversionType = "Miles to Kilometers";
    private const string RequiredTitle = "Conversions";
    private const double ExpectedKilometers = 5.055754176;
    private const double ConversionTolerance = 0.000001;

    [Fact]
    public async Task ConversionsPage_ReturnsSuccessStatusCode()
    {
        await using WebApplicationFactory<Program> application = new();
        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/Conversions", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ConversionsPage_DisplaysRequiredContent()
    {
        await using WebApplicationFactory<Program> application = new();
        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/Conversions", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains(RequiredConversionType, content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(RequiredInput, content, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Navigation_ContainsConversionsLink()
    {
        await using WebApplicationFactory<Program> application = new();
        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("href=\"/Conversions\"", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(">Conversions<", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ConversionsPageModel_ContainsRequiredProperties()
    {
        Type modelType = GetConversionsModelType();

        PropertyInfo? inputProperty = modelType.GetProperty("Input", BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo? outputProperty = modelType.GetProperty("Output", BindingFlags.Public | BindingFlags.Instance);

        Assert.NotNull(inputProperty);
        Assert.Equal(typeof(string), inputProperty.PropertyType);

        Assert.NotNull(outputProperty);
        Assert.Equal(typeof(string), outputProperty.PropertyType);
    }

    [Fact]
    public void OnGet_SetsRequiredInputAndViewData()
    {
        (PageModel model, Type modelType) = CreateAndRunPageModel();

        PropertyInfo inputProperty = GetRequiredProperty(modelType, "Input");
        string input = Assert.IsType<string>(inputProperty.GetValue(model));

        Assert.Equal(RequiredInput, input);
        Assert.Equal(RequiredConversionType, model.ViewData["ConversionType"]);
        Assert.Equal(RequiredTitle, model.ViewData["Title"]);
    }

    [Fact]
    public void OnGet_ProducesCorrectMilesToKilometersOutput()
    {
        (PageModel model, Type modelType) = CreateAndRunPageModel();

        PropertyInfo outputProperty = GetRequiredProperty(modelType, "Output");
        string output = Assert.IsType<string>(outputProperty.GetValue(model));

        Assert.True(
            TryParseDouble(output, out double actualKilometers),
            $"Output must contain a numeric value, but was '{output}'.");

        Assert.InRange(
            actualKilometers,
            ExpectedKilometers - ConversionTolerance,
            ExpectedKilometers + ConversionTolerance);
    }

    private static Type GetConversionsModelType()
    {
        Type? modelType = typeof(Program).Assembly
            .GetTypes()
            .SingleOrDefault(type =>
                type.Name == "ConversionsModel" &&
                typeof(PageModel).IsAssignableFrom(type));

        Assert.NotNull(modelType);
        return modelType;
    }

    private static (PageModel Model, Type ModelType) CreateAndRunPageModel()
    {
        Type modelType = GetConversionsModelType();
        object? instance = Activator.CreateInstance(modelType);

        PageModel model = Assert.IsAssignableFrom<PageModel>(instance);

        model.PageContext = new PageContext
        {
            ViewData = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
        };

        MethodInfo? onGet = modelType.GetMethod(
            "OnGet",
            BindingFlags.Public | BindingFlags.Instance,
            null,
            Type.EmptyTypes,
            null);

        Assert.NotNull(onGet);

        try
        {
            onGet.Invoke(model, null);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }

        return (model, modelType);
    }

    private static PropertyInfo GetRequiredProperty(Type modelType, string propertyName)
    {
        PropertyInfo? property = modelType.GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance);

        Assert.NotNull(property);
        return property;
    }

    private static bool TryParseDouble(string value, out double result)
    {
        if (double.TryParse(
                value,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.CurrentCulture,
                out result))
        {
            return true;
        }

        return double.TryParse(
            value,
            NumberStyles.Float | NumberStyles.AllowThousands,
            CultureInfo.InvariantCulture,
            out result);
    }
}
