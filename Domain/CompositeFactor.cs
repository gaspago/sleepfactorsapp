namespace SleepFactorsApp.Domain;

// Pattern: Composite pattern. A factor can contain many child factors and be handled uniformly through FactorBase.
public class CompositeFactor : FactorBase
{
    public override bool IsComposite => true;
}
