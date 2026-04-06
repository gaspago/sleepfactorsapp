namespace SleepFactorsApp.Domain;

public class DailyLog
{
    public int Id { get; set; }
    public DateOnly Day { get; set; }
    public SleepQuality SleepQuality { get; set; }
    public bool IsCommitted { get; set; } = true;
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public List<FactorBase> Factors { get; set; } = [];
}
