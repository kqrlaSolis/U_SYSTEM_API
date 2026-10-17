// Reglas de validación de los parámetros: tipo y rango por clave.
using System.Globalization;

namespace USystem.Application.Common;

public static class SettingRules
{
    // Rangos de los parámetros numéricos conocidos; el resto solo valida el tipo
    private static readonly Dictionary<string, (decimal Min, decimal Max)> _NumericRanges = new(
        StringComparer.Ordinal)
    {
        ["Login.MaxFailedAttempts"] = (1, 20),
        ["Login.LockoutMinutes"] = (1, 1440),
        ["Password.MinLength"] = (8, 128),
        ["Jwt.AccessTokenMinutes"] = (5, 1440),
        ["Jwt.RefreshTokenDays"] = (1, 365),
        ["PasswordReset.ExpirationMinutes"] = (5, 1440),
        ["Grades.MaxScore"] = (1, 100),
        ["Grades.MinPassingScore"] = (0, 100),
        ["Grades.MaxRecurrenceAttempts"] = (0, 10),
        ["Grades.MaxExtraordinaryAttempts"] = (0, 10),
        ["Grades.MaxRemedialAttempts"] = (0, 10),
        ["ReEnrollment.MaxSubjects"] = (1, 20),
        ["Payments.SurchargeValue"] = (0, 100),
        ["Payments.InstallmentsPerTerm"] = (1, 12),
        ["Storage.MaxFileSizeMb"] = (1, 100),
    };

    // Devuelve true cuando el valor respeta el tipo y el rango del parámetro
    public static bool IsValid(string SettingKey, string ValueType, string SettingValue)
    {
        var Value = SettingValue.Trim();

        switch (ValueType)
        {
            case "Int":
                return int.TryParse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var IntValue)
                    && IsInRange(SettingKey, IntValue);

            case "Decimal":
                return decimal.TryParse(
                        Value,
                        NumberStyles.Number,
                        CultureInfo.InvariantCulture,
                        out var DecimalValue)
                    && IsInRange(SettingKey, DecimalValue);

            case "Bool":
                return bool.TryParse(Value, out _) || Value == "0" || Value == "1";

            default:
                // String: cualquier valor no vacío
                return !string.IsNullOrWhiteSpace(Value);
        }
    }

    private static bool IsInRange(string SettingKey, decimal Value)
        => !_NumericRanges.TryGetValue(SettingKey, out var Range)
           || (Value >= Range.Min && Value <= Range.Max);
}
