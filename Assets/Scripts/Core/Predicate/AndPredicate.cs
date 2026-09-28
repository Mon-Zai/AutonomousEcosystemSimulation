using System;

public class AndPredicate : IPredicate
{
    private IPredicate[] predicates;

    public AndPredicate(params IPredicate[] predicates)
    {
        this.predicates = predicates;
    }
    public AndPredicate(params Func<bool>[] conditions)
    {
        predicates = new IPredicate[conditions.Length];
        for (int i = 0; i < conditions.Length; i++)
        {
            predicates[i] = new SimplePredicate(conditions[i]);
        }
    }

    public bool Evaluate()
    {
        foreach (var predicate in predicates)
        {
            if (!predicate.Evaluate())
            {
                return false;
            }
        }
        return true;
    }
}