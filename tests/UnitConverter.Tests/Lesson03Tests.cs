using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UnitConverter.Tests;

public class Lesson03Tests
{
    [Fact]
    public void ConversionModel_HasRequiredStringProperties()
    {
        Type? modelType = FindType("ConversionModel");

        Assert.NotNull(modelType);

        AssertStringProperty(modelType, "ConversionType");
        AssertStringProperty(modelType, "Input");
        AssertStringProperty(modelType, "Output");
    }

    [Fact]
    public void ConversionsPageModel_UsesConversionModel()
    {
        Type? conversionModelType = FindType("ConversionModel");

        Assert.NotNull(conversionModelType);

        Type pageModelType = Lesson02Tests.GetConversionsPageModelType();

        PropertyInfo? property = pageModelType
            .GetProperties()
            .SingleOrDefault(p =>
                p.PropertyType == conversionModelType);

        Assert.NotNull(property);
    }

    [Fact]
    public void ConversionModel_SupportsGetBinding()
    {
        Type? conversionModelType = FindType("ConversionModel");

        Assert.NotNull(conversionModelType);

        Type pageModelType = Lesson02Tests.GetConversionsPageModelType();

        PropertyInfo? property = pageModelType
            .GetProperties()
            .SingleOrDefault(p =>
                p.PropertyType == conversionModelType);

        Assert.NotNull(property);

        BindPropertyAttribute? attribute =
            property.GetCustomAttribute<BindPropertyAttribute>();

        Assert.NotNull(attribute);
        Assert.True(attribute.SupportsGet);
    }

    [Fact]
    public void ConversionTypes_DefinesRequiredConversions()
    {
        Type? type = FindType("ConversionTypes");

        Assert.NotNull(type);

        HashSet<string?> values = type.GetFields(
                BindingFlags.Public |
                BindingFlags.Static)
            .Where(field => field.FieldType == typeof(string))
            .Select(field => field.GetValue(null)?.ToString())
            .Where(value => value is not null)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("MilesToKilometers", values);
        Assert.Contains("KilometersToMiles", values);
        Assert.Contains("FahrenheitToCelsius", values);
        Assert.Contains("CelsiusToFahrenheit", values);
        Assert.Contains("PoundsToKilograms", values);
        Assert.Contains("KilogramsToPounds", values);
    }

    [Fact]
    public void ConversionTypes_ProvidesReadableDisplayNames()
    {
        Type? type = FindType("ConversionTypes");

        Assert.NotNull(type);

        IReadOnlyDictionary<string, string>? dictionary = GetConversionDictionary(type);

        Assert.NotNull(dictionary);

        AssertDictionaryEntry(dictionary, "MilesToKilometers", "Miles to Kilometers");
        AssertDictionaryEntry(dictionary, "KilometersToMiles", "Kilometers to Miles");
        AssertDictionaryEntry(dictionary, "FahrenheitToCelsius", "Fahrenheit to Celsius");
        AssertDictionaryEntry(dictionary, "CelsiusToFahrenheit", "Celsius to Fahrenheit");
        AssertDictionaryEntry(dictionary, "PoundsToKilograms", "Pounds to Kilograms");
        AssertDictionaryEntry(dictionary, "KilogramsToPounds", "Kilograms to Pounds");
    }

    [Fact]
    public async Task IndexPage_ContainsConversionForm()
    {
        await using WebApplicationFactory<Program> application = new();

        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("<form", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("method=\"get\"", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<select", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<input", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<label", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("name=\"Conversion.ConversionType\"", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("name=\"Conversion.Input\"", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task IndexPage_UsesRequiredBootstrapFormClasses()
    {
        await using WebApplicationFactory<Program> application = new();

        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("form-select", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("form-control", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("btn-primary", content, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("Miles to Kilometers")]
    [InlineData("Kilometers to Miles")]
    [InlineData("Fahrenheit to Celsius")]
    [InlineData("Celsius to Fahrenheit")]
    [InlineData("Pounds to Kilograms")]
    [InlineData("Kilograms to Pounds")]
    public async Task IndexPage_DisplaysRequiredConversion(string conversion)
    {
        await using WebApplicationFactory<Program> application = new();

        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains(conversion, content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task QueryStringConversion_ReturnsSuccessfulResult()
    {
        await using WebApplicationFactory<Program> application = new();

        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/Conversions?" +
                                                             "Conversion.ConversionType=MilesToKilometers&" +
                                                             "Conversion.Input=10",
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        string content =
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("16.09", content);
    }

    [Fact]
    public async Task PreviousRouteStyle_StillWorks()
    {
        await using WebApplicationFactory<Program> application = new();

        using HttpClient client = application.CreateClient();

        HttpResponseMessage response =
            await client.GetAsync("/Conversions/MilesToKilometers/10", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("16.09", content);
    }

    [Fact]
    public async Task ConversionsPage_HasReturnLink()
    {
        await using WebApplicationFactory<Program> application = new();

        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/Conversions?" +
                                                             "Conversion.ConversionType=MilesToKilometers&" +
                                                             "Conversion.Input=10",
            TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("href=\"/\"", content, StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string value)
    {
        return string.Concat(value.Where(c => !char.IsWhiteSpace(c))).ToLowerInvariant();
    }

    private static IReadOnlyDictionary<string, string>? GetConversionDictionary(Type conversionTypesType)
    {
        // Look for a public static property first.
        PropertyInfo? property = conversionTypesType
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(p =>
                typeof(IReadOnlyDictionary<string, string>)
                    .IsAssignableFrom(p.PropertyType));

        if (property?.GetValue(null) is IReadOnlyDictionary<string, string> propertyDictionary)
        {
            return propertyDictionary;
        }

        // Also allow a public static field.
        FieldInfo? field = conversionTypesType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(f =>
                typeof(IReadOnlyDictionary<string, string>)
                    .IsAssignableFrom(f.FieldType));

        if (field?.GetValue(null) is IReadOnlyDictionary<string, string> fieldDictionary)
        {
            return fieldDictionary;
        }

        return null;
    }

    private static void AssertDictionaryEntry(
        IReadOnlyDictionary<string, string> dictionary,
        string key,
        string displayName)
    {
        KeyValuePair<string, string> actual = dictionary
            .FirstOrDefault(entry =>
                string.Equals(entry.Key, key, StringComparison.OrdinalIgnoreCase));

        Assert.False(
            string.IsNullOrWhiteSpace(actual.Key),
            $"Conversion '{key}' was not found.");

        Assert.Equal(Normalize(displayName), Normalize(actual.Value));
    }

    private static Type? FindType(string name)
    {
        return typeof(Program).Assembly
            .GetTypes()
            .FirstOrDefault(type => type.Name == name);
    }

    private static void AssertStringProperty(Type type, string propertyName)
    {
        PropertyInfo? property = type.GetProperty(propertyName);

        Assert.NotNull(property);
        Assert.Equal(typeof(string), property.PropertyType);
        Assert.True(property.CanRead);
        Assert.True(property.CanWrite);
    }
}
