using SleepFactorsApp.Domain;

namespace SleepFactorsApp.Services;

// Pattern: Concrete Strategy. Ranks each factor independently con peso orario.
// Sera (1.5) > Pomeriggio (1.0) > Mattino (0.6) — la sera è più vicina al sonno.
public sealed class SingleFactorAnalysisStrategy : ISleepAnalysisStrategy
{
    public string Name => "single";

    public IReadOnlyList<AnalysisItem> Analyze(IReadOnlyList<DailyLog> logs)
    {
        // bucket: key -> (rawBad, rawSoSo, rawGood, rawTotal, wBad, wSoSo, wGood, wTotal)
        var bucket = new Dictionary<string, (int rBad, int rSoSo, int rGood, int rTotal, double wBad, double wSoSo, double wGood, double wTotal)>();

        foreach (var log in logs)
        {
            // Per ogni giorno, raggruppa per chiave e usa il peso del time slot più alto
            var keySlots = log.Factors
                .SelectMany(factor => factor.FlattenKeysWithTimeSlot())
                .GroupBy(item => item.Key)
                .Select(g => (Key: g.Key, Weight: g.Max(item => TimeWeight(item.Slot))))
                .ToList();

            foreach (var (key, weight) in keySlots)
            {
                bucket.TryGetValue(key, out var v);
                v.rTotal++;
                v.wTotal += weight;

                switch (log.SleepQuality)
                {
                    case SleepQuality.Bad:
                        v.rBad++; v.wBad += weight;
                        break;
                    case SleepQuality.SoSo:
                        v.rSoSo++; v.wSoSo += weight;
                        break;
                    case SleepQuality.Good:
                        v.rGood++; v.wGood += weight;
                        break;
                }

                bucket[key] = v;
            }
        }

        return bucket
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

    // Peso per fascia oraria: la sera influenza di più il sonno
    internal static double TimeWeight(TimeSlot slot) => slot switch
    {
        TimeSlot.Mattino => 0.6,
        TimeSlot.Pomeriggio => 1.0,
        TimeSlot.Sera => 1.5,
        _ => 1.0
    };

    private static double ComputeWeightedRisk(double wBad, double wSoSo, double wGood, double wTotal)
    {
        if (wTotal == 0) return 0;
        return Math.Round((wBad + (0.5 * wSoSo) - wGood) / wTotal, 4);
    }
}
