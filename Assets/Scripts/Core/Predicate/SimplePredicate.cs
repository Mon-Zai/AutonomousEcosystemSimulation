using System;

public class SimplePredicate : IPredicate
{
    private Func<bool> _condition;
    public SimplePredicate(Func<bool> condition)
    {
        _condition = condition;
    }
    public bool Evaluate()
    {
        return _condition();
    }
}