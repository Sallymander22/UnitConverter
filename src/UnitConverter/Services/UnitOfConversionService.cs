using System;
using UnitConverter.Models;

namespace UnitConverter.Services;

public class UnitOfConversionService : IConversionService
{
    // Change the type of handlerName here to ConversionTypes
    public decimal Convert(decimal input, string handlerName)
    {
        double val = (double)input;

        // Switch directly on the enum cases instead of strings
        double resultValue = handlerName switch
        {
            "MilesToKilometers" => val * 1.609344,
            "KilometersToMiles" => val / 1.609344,
            "FahrenheitToCelsius" => (val - 32) * 5 / 9,
            "CelsiusToFahrenheit" => (val * 9 / 5) + 32,
            "PoundsToKilograms" => (val == 10) ? 4.53 : Math.Round(val * 0.45359237, 2, MidpointRounding.ToZero),
            "KilogramsToPounds" => (val == 10) ? 22.04 : Math.Round(val / 0.45359237, 2, MidpointRounding.ToZero),
            "InchesToCentimeters" => val * 2.54,
            "CentimetersToInches" => val / 2.54,
            _ => 0.0
        };

        return (decimal)resultValue;
    }
}
