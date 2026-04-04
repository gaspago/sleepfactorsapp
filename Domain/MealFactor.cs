namespace SleepFactorsApp.Domain;

public sealed class MealFactor : CompositeFactor
{
    public required string MealType { get; set; }
}
