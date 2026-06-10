using JurisSeeker.Domain.Common.Enums;

namespace JurisSeeker.Infrastructure.Helpers;

public static class CaseOutcomeExtensions
{
    public static string ToDisplayString(this CaseOutcome outcome)
    {
        return outcome switch
        {
            CaseOutcome.Unknown => "Неизвестно",
            CaseOutcome.PlaintiffWon => "Истец выиграл",
            CaseOutcome.DefendantWon => "Ответчик выиграл",
            _ => "Неизвестно"
        };
    }
    
    public static CaseOutcome ToDisplayEnum(this string outcome)
    {
        return outcome switch
        {
            "Неизвестно" => CaseOutcome.Unknown,
            "Истец выиграл" => CaseOutcome.PlaintiffWon,
            "Ответчик выиграл" => CaseOutcome.DefendantWon,
            _ => CaseOutcome.Unknown
        };
    }
}