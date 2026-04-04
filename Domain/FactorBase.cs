namespace SleepFactorsApp.Domain;

// Pattern: Base Class pattern. Common behavior and structure for every factor type.
public abstract class FactorBase
{
    public int Id { get; set; }
    public int DailyLogId { get; set; }
    public DailyLog? DailyLog { get; set; }

    public int? ParentFactorId { get; set; }
    public FactorBase? ParentFactor { get; set; }
    public List<FactorBase> Children { get; set; } = [];

    public required string Name { get; set; }
    public string? Detail { get; set; }
    public FactorCategory Category { get; set; }

    public abstract bool IsComposite { get; }

    public virtual string Key => $"{Category}:{Name}".Trim().ToLowerInvariant();

    public virtual IEnumerable<string> FlattenKeys()
    {
        yield return Key;

        foreach (var child in Children)
        {
            foreach (var childKey in child.FlattenKeys())
            {
                yield return childKey;
            }
        }
    }
}
