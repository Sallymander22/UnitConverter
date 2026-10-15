using UnitConverter.Models;

namespace UnitConverter.Services;

public interface IConversionService
{
    decimal Convert(decimal input, string handlerName);
}
