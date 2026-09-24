using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UnitConverter;

public class ConversionModel
{
    public string ConversionType { get; set; } = "";
    public string Input { get; set; } = "";
    public string Output { get; set; } = "";



}
