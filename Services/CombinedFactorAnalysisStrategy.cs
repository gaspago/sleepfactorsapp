using SleepFactorsApp.Domain;

namespace SleepFactorsApp.Services;

// Pattern: Concrete Strategy. Ranks factor combinations (pairs) con peso orario medio della coppia.
public sealed class CombinedFactorAnalysisStrategy : ISleepAnalysisStrategy
{
    public string Name => "combined";

    public IReadOnlyList<AnalysisItem> Analyze(IReadOnlyList<DailyLog> logs)
    {
        var bucket = new Dictionary<string, (int rBad, int rSoSo, int rGood, int rTotal, double wBad, double wSoSo, double wGood, double wTotal)>();

        foreach (var log in logs)
        {
            var entries = log.Factors
                .SelectMany(factor => factor.FlattenKeysWithTimeSlot())
                .GroupBy(item => item.Key)
                .Select(g => (Key: g.Key, Weight: g.Max(item => SingleFactorAnalysisStrategy.TimeWeight(item.Slot))))
                .OrderBy(item => item.Key)
                .ToList();

            for (var i = 0; i < entries.Count; i++)
            {
                for (var j = i + 1; j < entries.Count; j++)
                {
                    var pair = $"{entries[i].Key} + {entries[j].Key}";
                    // Peso della coppia = media dei pesi individuali
                    var pairWeight = (entries[i].Weight + entries[j].Weight) / 2.0;

                    bucket.TryGetValue(pair, out var v);
                    v.rTotal++;
                    v.wTotal += pairWeight;

                    switch (log.SleepQuality)
                    {
                        case SleepQuality.Bad:
                            v.rBad++; v.wBad += pairWeight;
                            break;
                        case SleepQuality.SoSo:
                            v.rSoSo++; v.wSoSo += pairWeight;
                            break;
                        case SleepQuality.Good:
                            v.rGood++; v.wGood += pairWeight;
                            break;
                    }

                    bucket[pair] = v;
                }
            }
        }

        return bucket
            .Where(item => item.Value.rTotal >= 2)
            .Select(item => new AnalysisItem(
                item.Key,
                item.Value.rTotal,
                item.Value.rBad,
                item.Value.rSoSo,
                item.Value.rGood,
                ComputeWeightedRisk(item.Value.wBad, item.Value.wSoSo, item.Value.wGood, item.Value.wTotal)))
            .OrderByDescending(item => item.RiskScore)
            .ThenByDescending(item => item.Occurrences)
            .ToList();
    }

    private static double ComputeWeightedRisk(double wBad, double wSoSo, double wGood, double wTotal)
    {
        if (wTotal == 0) return 0;
        return Math.Round((wBad + (0.5 * wSoSo) - wGood) / wTotal, 4);
    }
}
