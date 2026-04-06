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

public sealed record AnalysisItem(string Key, int Occurrences, int BadCount, int SoSoCount, int GoodCount, double RiskScore);

public sealed record AnalysisReport(
    IReadOnlyList<AnalysisItem> SingleFactorRanking,
    IReadOnlyList<AnalysisItem> CombinedFactorRanking,
    IReadOnlyList<AnalysisItem> ExcludedFactors,
    int TotalDays);
