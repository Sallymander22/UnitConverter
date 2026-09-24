using System.Collections.Generic;

namespace UnitConverter;

public static class ConversionTypes
{
    public const string MilesToKilometers = "MilesToKilometers";
    public const string KilometersToMiles = "KilometersToMiles";
    public const string FahrenheitToCelsius = "FahrenheitToCelsius";
    public const string CelsiusToFahrenheit = "CelsiusToFahrenheit";
    public const string PoundsToKilograms = "PoundsToKilograms";
    public const string KilogramsToPounds = "KilogramsToPounds";
    public const string InchesToCentimeters = "InchesToCentimeters";
    public const string CentimetersToInches = "CentimetersToInches";

    public static readonly Dictionary<string, string> SupportedTypes = new Dictionary<string, string>
    {
        { MilesToKilometers, "Miles to Kilometers" },
        { KilometersToMiles, "Kilometers to Miles" },
        { FahrenheitToCelsius, "Fahrenheit to Celsius" },
        { CelsiusToFahrenheit, "Celsius to Fahrenheit" },
        { PoundsToKilograms, "Pounds to Kilograms" },
        { KilogramsToPounds, "Kilograms to Pounds" },
        { InchesToCentimeters, "Inches to Centimeters" },
        { CentimetersToInches, "Centimeters to Inches" }

    };
}
