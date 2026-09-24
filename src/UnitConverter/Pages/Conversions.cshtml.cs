using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace UnitConverter.Pages;

public class ConversionsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public ConversionModel Conversion { get; set; } = new ConversionModel();

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
            ConversionTypes.SupportedTypes.TryGetValue(Conversion.ConversionType, out var displayName)
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
            ConversionTypes.MilesToKilometers => new UnitOf.Length().FromMiles(parsedInput).ToKilometers().ToString(),
            ConversionTypes.KilometersToMiles => new UnitOf.Length().FromKilometers(parsedInput).ToMiles().ToString(),

            ConversionTypes.FahrenheitToCelsius => new UnitOf.Temperature().FromFahrenheit(parsedInput).ToCelsius()
                .ToString(),
            ConversionTypes.CelsiusToFahrenheit => new UnitOf.Temperature().FromCelsius(parsedInput).ToFahrenheit()
                .ToString(),

            ConversionTypes.PoundsToKilograms => new UnitOf.Mass().FromPounds(parsedInput).ToKilograms().ToString(),
            ConversionTypes.KilogramsToPounds => new UnitOf.Mass().FromKilograms(parsedInput).ToPounds().ToString(),

            ConversionTypes.InchesToCentimeters => new UnitOf.Length().FromInches(parsedInput).ToCentimeters()
                .ToString(),
            ConversionTypes.CentimetersToInches => new UnitOf.Length().FromCentimeters(parsedInput).ToInches()
                .ToString(),

            _ => string.Empty
        };

        if (string.IsNullOrEmpty(Conversion.Output))
        {
            ViewData["ErrorMessage"] = "Unknown conversion type";
        }
    }
}
