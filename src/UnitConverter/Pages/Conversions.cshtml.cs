using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnitConverter.Models;
using UnitOf;

namespace UnitConverter.Pages;

public class ConversionsModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public ConversionModel Conversion { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Input
    {
        get => Conversion?.Input;
        set { if (Conversion != null) Conversion.Input = value; }
    }

    public string? Output
    {
        get => Conversion?.Output;
        set { if (Conversion != null) Conversion.Output = value; }
    }

    [BindProperty(SupportsGet = true)]
    public string? ConversionType
    {
        get => Conversion?.ConversionType;
        set { if (Conversion != null) Conversion.ConversionType = value; }
    }

    public double? Value
    {
        get => double.TryParse(Conversion?.Input, out var parsed) ? parsed : null;
        set { if (Conversion != null) Conversion.Input = value?.ToString() ?? string.Empty; }
    }

    public double? ConvertedValue
    {
        get => double.TryParse(Conversion?.Output, out var parsed) ? parsed : null;
        set { if (Conversion != null) Conversion.Output = value?.ToString() ?? string.Empty; }
    }

    public void OnGet()
    {
        if (string.IsNullOrEmpty(Conversion.ConversionType))
        {
            Conversion.ConversionType = "MilesToKilometers";
        }

        if (string.IsNullOrEmpty(Conversion.Input))
        {
            Conversion.Input = "3.1415";
        }

        ViewData["Title"] = "Conversions";

        ViewData["ConversionType"] =
            ConversionTypes.All.TryGetValue(Conversion.ConversionType, out string? displayName)
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

        Conversion.Output = Conversion.ConversionType switch
        {
            "MilesToKilometers" => new Length().FromMiles(parsedInput).ToKilometers().ToString(),
            "KilometersToMiles" => new Length().FromKilometers(parsedInput).ToMiles().ToString(),

            "FahrenheitToCelsius" => new Temperature().FromFahrenheit(parsedInput).ToCelsius().ToString(),
            "CelsiusToFahrenheit" => new Temperature().FromCelsius(parsedInput).ToFahrenheit().ToString(),

            "PoundsToKilograms" => new Mass().FromPounds(parsedInput).ToKilograms().ToString(),
            "KilogramsToPounds" => new Mass().FromKilograms(parsedInput).ToPounds().ToString(),

            "InchesToCentimeters" => new Length().FromInches(parsedInput).ToCentimeters().ToString(),
            "CentimetersToInches" => new Length().FromCentimeters(parsedInput).ToInches().ToString(),

            _ => string.Empty
        };

        if (string.IsNullOrEmpty(Conversion.Output))
        {
            ViewData["ErrorMessage"] = "Unknown conversion type";
        }
    }
}
