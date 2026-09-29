using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using UnitConverter;

namespace UnitConverter.Pages
{
    public class QuickConversionsModel : PageModel
    {
        public IEnumerable<SelectListItem> PoundOptions =>
        [
            new("1 pound", "1"),
            new("5 pounds", "5"),
            new("10 pounds", "10"),
            new("25 pounds", "25"),
            new("50 pounds", "50")
        ];

        public void OnGet()
        {
        }

        private IActionResult RedirectToConversion(string conversionType, string input)
        {
            return RedirectToPage("/Conversions", new { type = conversionType, value = input });

        }

        public IActionResult OnGetMilesToKilometers(string input) => RedirectToConversion(ConversionTypes.MilesToKilometers, input);
        public IActionResult OnGetKilometersToMiles(string input) => RedirectToConversion(ConversionTypes.KilometersToMiles, input);
        public IActionResult OnGetFahrenheitToCelsius(string input) => RedirectToConversion(ConversionTypes.FahrenheitToCelsius, input);
        public IActionResult OnGetCelsiusToFahrenheit(string input) => RedirectToConversion(ConversionTypes.CelsiusToFahrenheit, input);
        public IActionResult OnGetPoundsToKilograms(string input) => RedirectToConversion(ConversionTypes.PoundsToKilograms, input);
        public IActionResult OnGetKilogramsToPounds(string input) => RedirectToConversion(ConversionTypes.KilogramsToPounds, input);

        public IActionResult OnGetInchesToCentimeters(string input) => RedirectToConversion(ConversionTypes.InchesToCentimeters, input);
        public IActionResult OnGetCentimetersToInches(string input) => RedirectToConversion(ConversionTypes.CentimetersToInches, input);
    }
}

