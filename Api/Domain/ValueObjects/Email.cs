using Vogen;

namespace Api.Domain.ValueObjects;

[ValueObject<string>(conversions: Conversions.EfCoreValueConverter | Conversions.SystemTextJson)]
public partial class Email
{
    private static Validation Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Validation.Invalid("El correo electrónico no puede estar vacío.");

        if (!value.Contains("@"))
            return Validation.Invalid("El formato del correo electrónico no es válido.");

        return Validation.Ok;
    }
}
