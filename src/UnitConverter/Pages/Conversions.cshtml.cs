using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UnitConverter.Models;
using UnitOf;

namespace UnitConverter.Pages;

public class ConversionsModel : PageModel
{
    private readonly ILogger<ConversionsModel>? _logger;

    [ActivatorUtilitiesConstructor]
    public ConversionsModel(ILogger<ConversionsModel> logger)
    {
        _logger = logger;
    }

    public ConversionsModel()
    {
        _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<ConversionsModel>.Instance;
    }

    [BindProperty(SupportsGet = true)]
    public ConversionModel Conversion { get; set; } = new();

    // --- LESSON 1 & 2 REGRESSION BRIDGE PROPERTIES ---
    [BindProperty(Name = "Input", SupportsGet = true)]
    [BindNever]
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

    [BindProperty(Name = "ConversionType", SupportsGet = true)]
    [BindNever]
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
        if (Conversion == null)
        {
            Conversion = new ConversionModel();
        }

        if (string.IsNullOrEmpty(Conversion.ConversionType) && !string.IsNullOrEmpty(ConversionType))
        {
            Conversion.ConversionType = ConversionType;
        }

        if (string.IsNullOrEmpty(Conversion.Input) && !string.IsNullOrEmpty(Input))
        {
            Conversion.Input = Input;
        }

        if (string.IsNullOrEmpty(Conversion.ConversionType))
        {
            Conversion.ConversionType = "MilesToKilometers";
        }

        if (string.IsNullOrEmpty(Conversion.Input))
        {
            Conversion.Input = "3.1415";
        }

        ViewData["Title"] = "Conversions";

        if (ConversionTypes.All.TryGetValue(Conversion.ConversionType, out string? displayName))
        {
            ViewData["ConversionType"] = displayName ?? Conversion.ConversionType;
        }
        else
        {
            ViewData["ConversionType"] = Conversion.ConversionType;
        }

        double parsedInput = 0;

        try
        {
            parsedInput = Convert.ToDouble(Conversion.Input);
        }
        catch (FormatException)
        {
            _logger?.LogWarning("Invalid input format encountered: {Input}", Conversion.Input);
            ViewData["ErrorMessage"] = "Input must be a valid number";
            return;
        }

        try
        {
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
                _logger?.LogWarning("Unknown conversion type attempted: {ConversionType}", Conversion.ConversionType);
                ViewData["ErrorMessage"] = "Unknown conversion type";
                return;
            }

            _logger?.LogInformation(
                "Converted {Input} using {ConversionType} with result {Result}",
                Conversion.Input,
                Conversion.ConversionType,
                Conversion.Output);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An unexpected conversion failure occurred for type {ConversionType}", Conversion.ConversionType);
            ViewData["ErrorMessage"] = "An error occurred during calculation";
        }
    }
}
