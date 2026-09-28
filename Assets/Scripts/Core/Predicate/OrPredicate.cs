public class OrPredicate : IPredicate
{
    private IPredicate[] predicates;

    public OrPredicate(params IPredicate[] predicates)
    {
        this.predicates = predicates;
    }

    public bool Evaluate()
    {
        foreach (var predicate in predicates)
        {
            if (predicate.Evaluate())
            {
                return true;
            }
        }
        return false;
    }
}