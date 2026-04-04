using SleepFactorsApp.Domain;

namespace SleepFactorsApp.Services;

// Pattern: Strategy pattern. Different analysis algorithms share the same contract.
public interface ISleepAnalysisStrategy
{
    string Name { get; }
    IReadOnlyList<AnalysisItem> Analyze(IReadOnlyList<DailyLog> logs);
}
