using System;

public class StateTransition<T>
{
    private IPredicate _predicate;

    public T TargetKey { get; private set; }

    public StateTransition(T targetKey)
    {
        TargetKey = targetKey;
    }
    public StateTransition<T> SetPredicate(IPredicate predicate)
    {
        _predicate = predicate;
        return this;
    }
    public bool Evaluate()
    {
        return _predicate != null ? _predicate.Evaluate() : true;
    }
}