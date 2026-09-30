using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnitConverter.Models;
using UnitOf;

namespace UnitConverter.Pages;

public class ConversionsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public ConversionModel Conversion { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string ConversionType
    {
        get => Conversion.ConversionType;
        set => Conversion.ConversionType = value;
    }

    [BindProperty(SupportsGet = true)]
    public string Input
    {
        get => Conversion.Input;
        set => Conversion.Input = value;
    }

    public string Output
    {
        get => Conversion.Output;
        set => Conversion.Output = value;
    }

    public void OnGet()
    {
        if (string.IsNullOrEmpty(Conversion.ConversionType))
        {
            Conversion.ConversionType = ConversionTypes.MilesToKilometers;
        }

        if (string.IsNullOrEmpty(Conversion.Input))
        {
            Conversion.Input = "3.1415";
        }

        ViewData["Title"] = "Conversions";

        // Fixes the multi-line lookup error
        ViewData["ConversionType"] =
            ConversionTypes.SupportedTypes.TryGetValue(Conversion.ConversionType, out string? displayName)
                ? displayName
                : Conversion.ConversionType;

        double parsedInput = 0;

        try
        {
            parsedInput = Convert.ToDouble(Conversion.Input);
        }
        catch (FormatException)
        {
            ViewData["ErrorMessage"] = "Input must be a valid number";
            return;
        }

        // Fixes the switch statement mapping issues
        Conversion.Output = Conversion.ConversionType switch
        {
            ConversionTypes.MilesToKilometers => new Length().FromMiles(parsedInput).ToKilometers().ToString(),
            ConversionTypes.KilometersToMiles => new Length().FromKilometers(parsedInput).ToMiles().ToString(),

            ConversionTypes.FahrenheitToCelsius => new Temperature().FromFahrenheit(parsedInput).ToCelsius()
                .ToString(),
            ConversionTypes.CelsiusToFahrenheit => new Temperature().FromCelsius(parsedInput).ToFahrenheit()
                .ToString(),

            ConversionTypes.PoundsToKilograms => new Mass().FromPounds(parsedInput).ToKilograms().ToString(),
            ConversionTypes.KilogramsToPounds => new Mass().FromKilograms(parsedInput).ToPounds().ToString(),

            ConversionTypes.InchesToCentimeters => new Length().FromInches(parsedInput).ToCentimeters()
                .ToString(),
            ConversionTypes.CentimetersToInches => new Length().FromCentimeters(parsedInput).ToInches()
                .ToString(),

            _ => string.Empty
        };

        if (string.IsNullOrEmpty(Conversion.Output))
        {
            ViewData["ErrorMessage"] = "Unknown conversion type";
        }
    }
}
