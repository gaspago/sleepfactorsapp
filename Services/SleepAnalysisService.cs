using SleepFactorsApp.Domain;

namespace SleepFactorsApp.Services;

public sealed class SleepAnalysisService(DailyLogService dailyLogService)
{
    private readonly SingleFactorAnalysisStrategy _singleStrategy = new();
    private readonly CombinedFactorAnalysisStrategy _combinedStrategy = new();

    public async Task<AnalysisReport> BuildReportAsync(CancellationToken cancellationToken = default)
    {
        var logs = await dailyLogService.GetAllLogsAsync(cancellationToken);
        var single = _singleStrategy.Analyze(logs);
        var combined = _combinedStrategy.Analyze(logs);

        var excluded = single
            .Where(item => item.GoodCount >= item.BadCount)
            .OrderByDescending(item => item.Occurrences)
            .ToList();

        var singleRanking = single
            .Where(item => item.BadCount > item.GoodCount)
            .ToList();

        var combinedRanking = combined
            .Where(item => item.BadCount > item.GoodCount)
            .ToList();

        return new AnalysisReport(singleRanking, combinedRanking, excluded, logs.Count);
    }

    public static string ExtraIdea =>
        "Extra idea: add a 1-day lag analysis (factor happened yesterday, sleep quality tonight) to detect delayed effects.";
}
