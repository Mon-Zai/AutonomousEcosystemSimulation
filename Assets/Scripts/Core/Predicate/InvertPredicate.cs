public class InvertPredicate : IPredicate
{
    private IPredicate _predicate;

    public InvertPredicate(IPredicate predicate)
    {
        _predicate = predicate;
    }

    public bool Evaluate()
    {
        return !_predicate.Evaluate();
    }
}