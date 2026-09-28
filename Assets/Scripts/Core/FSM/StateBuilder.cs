using System;

public class StateBuilder<T>
{
    private readonly StateMachine<T> _fsm;
    private readonly T _key;

    public StateBuilder(StateMachine<T> fsm, T key)
    {
        _fsm = fsm;
        _key = key;
    }

    public StateBuilder<T> TransitionTo(T target, IPredicate predicate,
         TransitionContext context = TransitionContext.Update)
    {
        var transition = new StateTransition<T>(target).SetPredicate(predicate);
        _fsm.AddTransition(_key, transition, context);
        return this;
    }
    public StateBuilder<T> TriggeredTransitionTo(T target, string trigger, IPredicate predicate)
    {
        var transition = new StateTransition<T>(target).SetPredicate(predicate);
        _fsm.AddTriggeredTransition(_key, trigger, transition);
        return this;
    }
}