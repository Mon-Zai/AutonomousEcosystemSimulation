using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    IState _currentState;
    T _currentKey;
    private TransitionHandler<T> _transitionHandler;
    private Dictionary<T, IState> _states = new Dictionary<T, IState>();
    public T CurrentKey => _currentKey;
    public IState CurrentState => _currentState;

    public StateMachine()
    {
        _transitionHandler = new TransitionHandler<T>(this);
    }
    public void Update()
    {
        _currentState?.Update();
        _transitionHandler?.CheckTransitions(TransitionContext.Update, _currentKey);
    }
    public void FixedUpdate()
    {
        _currentState?.FixedUpdate();
        _transitionHandler?.CheckTransitions(TransitionContext.FixedUpdate, _currentKey);
    }

    public StateBuilder<T> AddState(T key, IState state)
    {
        _states[key] = state;
        _transitionHandler.RegisterState(key);
        return new StateBuilder<T>(this, key);
    }
    public void ChangeState(T key)
    {
        if (!_states.TryGetValue(key, out IState newState))
        {
            Debug.LogError($"State {key} not found.");
            return;
        }
        if (_currentState != null && EqualityComparer<T>.Default.Equals(_currentKey, key)) return;

        _currentState?.OnExit();
        _currentKey = key;
        _currentState = newState;
        _currentState.OnEnter();
    }

    public void AddTransition(T from, StateTransition<T> transition, TransitionContext context)
    => _transitionHandler.AddTransition(from, transition, context);

    public void AddTriggeredTransition(T from, string trigger, StateTransition<T> transition)
        => _transitionHandler.AddTriggeredTransition(from, trigger, transition);

    public void Trigger(string trigger)
        => _transitionHandler.Trigger(trigger, _currentKey);
}