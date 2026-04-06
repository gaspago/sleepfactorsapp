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

    // Momento della giornata — usato come peso nell'analisi del rischio
    public TimeSlot TimeSlot { get; set; } = TimeSlot.Pomeriggio;

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

    // Restituisce ogni chiave con il suo time slot per il calcolo del rischio ponderato
    // I figli (ingredienti) ereditano il time slot del genitore
    public virtual IEnumerable<(string Key, TimeSlot Slot)> FlattenKeysWithTimeSlot()
    {
        yield return (Key, TimeSlot);

        foreach (var child in Children)
        {
            yield return (child.Key, TimeSlot);
        }
    }
}
