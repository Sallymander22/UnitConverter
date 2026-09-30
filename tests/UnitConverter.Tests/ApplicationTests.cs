using Microsoft.AspNetCore.Mvc.Testing;

namespace UnitConverter.Tests;

public class ApplicationTests
{
    [Fact]
    public async Task HomePage_ReturnsSuccessStatusCode()
    {
        await using WebApplicationFactory<Program> application = new();

        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task HomePage_ContainsExpectedHeading()
    {
        await using WebApplicationFactory<Program> application = new();

        using HttpClient client = application.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/", TestContext.Current.CancellationToken);
        string content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("Unit Converter", content);
    }
}
