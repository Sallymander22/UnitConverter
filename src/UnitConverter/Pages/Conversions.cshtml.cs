using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UnitConverter.Pages;

public class ConversionsModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string ConversionType { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; }  = string.Empty;

    public void OnGet()
    {
        if (string.IsNullOrEmpty(ConversionType))
        {
            ConversionType = "MilesToKilometers";
        }

        if (string.IsNullOrEmpty(Input))
        {
            Input = "3.1415";
        }

        ViewData["ConversionType"] = ConversionType;
        ViewData["Title"] = "Conversions";

        if (ConversionType == "MilesToKilometers")
        {
            ViewData["ConversionType"] = "Miles to Kilometers";
        }
        else
        {
            ViewData["ConversionType"] = ConversionType;
        }

        double parsedInput = 0;

        try
        {
            parsedInput = Convert.ToDouble(Input);
        }
        catch (FormatException)
        {
            ViewData["ErrorMessage"] = "Input must be a number";
            return;
        }

        Output = ConversionType switch
        {
            "MilesToKilometers" => new UnitOf.Length().FromMiles(parsedInput).ToKilometers().ToString(),
            "KilometersToMiles" => new UnitOf.Length().FromKilometers(parsedInput).ToMiles().ToString(),

            "FahrenheitToCelsius" => new UnitOf.Temperature().FromFahrenheit(parsedInput).ToCelsius().ToString(),
            "CelsiusToFahrenheit" => new UnitOf.Temperature().FromCelsius(parsedInput).ToFahrenheit().ToString(),

            "PoundsToKilograms" => new UnitOf.Mass().FromPounds(parsedInput).ToKilograms().ToString(),
            "KilogramsToPounds" => new UnitOf.Mass().FromKilograms(parsedInput).ToPounds().ToString(),

            "InchesToCentimeters" => new UnitOf.Length().FromInches(parsedInput).ToCentimeters().ToString(),
            "CentimetersToInches" => new UnitOf.Length().FromCentimeters(parsedInput).ToInches().ToString(),

            _ => string.Empty
        };

        if (string.IsNullOrEmpty(Output))
        {
            ViewData["ErrorMessage"] = "Unknown conversion type";
        }

    }
}
