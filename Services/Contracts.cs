using SleepFactorsApp.Domain;

namespace SleepFactorsApp.Services;

public sealed record SimpleFactorInput(FactorCategory Category, string Name, string? Detail, TimeSlot TimeSlot = TimeSlot.Pomeriggio);

public sealed record MealFactorInput(string MealType, IReadOnlyList<string> Ingredients, string? Detail, TimeSlot TimeSlot = TimeSlot.Pomeriggio);

public sealed record DailyLogInput(
    DateOnly Day,
    SleepQuality SleepQuality,
    string? Notes,
    IReadOnlyList<SimpleFactorInput> SimpleFactors,
    IReadOnlyList<MealFactorInput> MealFactors);

public sealed record DailyDraftInput(
    DateOnly Day,
    string? Notes,
    IReadOnlyList<SimpleFactorInput> SimpleFactors,
    IReadOnlyList<MealFactorInput> MealFactors);

public sealed record CommitSleepInput(
    DateOnly Day,
    SleepQuality SleepQuality,
    string? Notes);

public sealed record DayCollectorStatus(
    DateOnly Day,
    int LogsCount,
    int TotalFactors,
    bool HasDraft,
    bool HasCommitted);

public sealed record AnalysisItem(string Key, int Occurrences, int BadCount, int SoSoCount, int GoodCount, double RiskScore);

public sealed record AnalysisReport(
    IReadOnlyList<AnalysisItem> SingleFactorRanking,
    IReadOnlyList<AnalysisItem> CombinedFactorRanking,
    IReadOnlyList<AnalysisItem> ExcludedFactors,
    int TotalDays);
