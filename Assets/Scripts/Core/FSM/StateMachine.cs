using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    State _currentState;
    T _currentKey;
    private TransitionHandler<T> _transitionHandler;
    private Dictionary<T, State> _states = new Dictionary<T, State>();
    public T CurrentKey => _currentKey;
    public State CurrentState => _currentState;

    public StateMachine()
    {
        _transitionHandler = new TransitionHandler<T>(this);
    }
    public void Update()
    {
        _currentState?.InvokeOnUpdate();
        _transitionHandler?.CheckTransitions(TransitionContext.Update, _currentKey);
    }
    public void FixedUpdate()
    {
        _currentState?.InvokeOnFixedUpdate();
        _transitionHandler?.CheckTransitions(TransitionContext.FixedUpdate, _currentKey);
    }

    public StateBuilder<T> AddState(T key, State state)
    {
        _states[key] = state;
        _transitionHandler.RegisterState(key);
        return new StateBuilder<T>(this, key);
    }
    public void ChangeState(T key)
    {
        if (!_states.TryGetValue(key, out State newState))
        {
            Debug.LogError($"State {key} not found.");
            return;
        }
        if (_currentState != null && EqualityComparer<T>.Default.Equals(_currentKey, key)) return;

        _currentState?.InvokeOnExit();
        _currentKey = key;
        _currentState = newState;
        _currentState.InvokeOnEnter();
    }
    public bool TryGetState(T key, out State state)
    {
        return _states.TryGetValue(key, out state);
    }
    public void AddTransition(T from, StateTransition<T> transition, TransitionContext context)
    => _transitionHandler.AddTransition(from, transition, context);

    public void AddTriggeredTransition(T from, string trigger, StateTransition<T> transition)
        => _transitionHandler.AddTriggeredTransition(from, trigger, transition);

    public void Trigger(string trigger)
        => _transitionHandler.Trigger(trigger, _currentKey);
}