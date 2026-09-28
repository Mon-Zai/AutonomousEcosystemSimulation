using System.Collections.Generic;
using UnityEngine;

public enum TransitionContext
{
    Update,
    FixedUpdate
}
public class TransitionHandler<T>
{
    private readonly Dictionary<T, List<StateTransition<T>>> _transitions = new();
    private readonly Dictionary<T, List<StateTransition<T>>> _fixedTransitions = new();
    private readonly Dictionary<T, Dictionary<string, StateTransition<T>>> _triggeredTransitions = new();
    private StateMachine<T> _stateMachine;
    public void RegisterState(T key)
    {
        _transitions[key] = new List<StateTransition<T>>();
        _fixedTransitions[key] = new List<StateTransition<T>>();
        _triggeredTransitions[key] = new Dictionary<string, StateTransition<T>>();
    }
    public TransitionHandler(StateMachine<T> stateMachine)
    {
        _stateMachine = stateMachine;
    }
    public void AddTransition(T from, StateTransition<T> transition, TransitionContext context = TransitionContext.Update)
    {
        var targetDictionary = GetListByContext(context);

        if (targetDictionary == null)
        {
            Debug.LogWarning($"Unknown transition context: {context}");
            return;
        }

        if (!targetDictionary.ContainsKey(from))
        {
            Debug.LogWarning($"State {from} not registered.");
            return;
        }

        targetDictionary[from].Add(transition);
    }
    public void AddTriggeredTransition(T from, string trigger, StateTransition<T> transition)
    {
        if (!_triggeredTransitions.ContainsKey(from))
        {
            Debug.LogWarning($"State {from} not registered.");
            return;
        }
        _triggeredTransitions[from][trigger] = transition;
    }
    public void Trigger(string trigger, T _currentKey)
    {
        if (!_triggeredTransitions.TryGetValue(_currentKey, out var triggers)) return;
        if (!triggers.TryGetValue(trigger, out var transition)) return;

        if (transition.Evaluate())
            _stateMachine.ChangeState(transition.TargetKey);
    }

    public void CheckTransitions(TransitionContext context, T currentKey)
    {
        var transitions = GetListByContext(context);
        if (!transitions.TryGetValue(currentKey, out var stateTransitions)) return;
        foreach (var transition in stateTransitions)
        {
            if (transition.Evaluate())
            {
                _stateMachine.ChangeState(transition.TargetKey);
                break;
            }
        }
    }
    private Dictionary<T, List<StateTransition<T>>> GetListByContext(TransitionContext context)
    {
        var targetDictionary = context switch
        {
            TransitionContext.Update => _transitions,
            TransitionContext.FixedUpdate => _fixedTransitions,
            _ => null
        };

        return targetDictionary;
    }
}