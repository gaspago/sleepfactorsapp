using SleepFactorsApp.Domain;

namespace SleepFactorsApp.Services;

// Pattern: Concrete Strategy. Ranks factor combinations (pairs) to detect interactions.
public sealed class CombinedFactorAnalysisStrategy : ISleepAnalysisStrategy
{
    public string Name => "combined";

    public IReadOnlyList<AnalysisItem> Analyze(IReadOnlyList<DailyLog> logs)
    {
        var bucket = new Dictionary<string, (int total, int bad, int soSo, int good)>();

        foreach (var log in logs)
        {
            var uniqueKeys = log.Factors
                .SelectMany(factor => factor.FlattenKeys())
                .Distinct()
                .OrderBy(key => key)
                .ToList();

            for (var i = 0; i < uniqueKeys.Count; i++)
            {
                for (var j = i + 1; j < uniqueKeys.Count; j++)
                {
                    var pair = $"{uniqueKeys[i]} + {uniqueKeys[j]}";
                    bucket.TryGetValue(pair, out var value);
                    value.total++;

                    switch (log.SleepQuality)
                    {
                        case SleepQuality.Bad:
                            value.bad++;
                            break;
                        case SleepQuality.SoSo:
                            value.soSo++;
                            break;
                        case SleepQuality.Good:
                            value.good++;
                            break;
                    }

                    bucket[pair] = value;
                }
            }
        }

        return bucket
            .Where(item => item.Value.total >= 2)
            .Select(item => new AnalysisItem(
                item.Key,
                item.Value.total,
                item.Value.bad,
                item.Value.soSo,
                item.Value.good,
                ComputeRisk(item.Value.bad, item.Value.soSo, item.Value.good, item.Value.total)))
            .OrderByDescending(item => item.RiskScore)
            .ThenByDescending(item => item.Occurrences)
            .ToList();
    }

    private static double ComputeRisk(int bad, int soSo, int good, int total)
    {
        if (total == 0)
        {
            return 0;
        }

        return Math.Round((bad + (0.5 * soSo) - good) / total, 4);
    }
}
