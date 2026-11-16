using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeedState : ParentBaseState
{
    private readonly AnimalBlackboard _blackboard;
    private readonly AnimalController _controller;
    private readonly Func<NeedType, bool> _canHandleMissingResource;
    private readonly Dictionary<NeedType, BaseState> _states;
    public HungryState Hungry { get; }
    public NeedType? CurrentNeed { get; private set; }
    public bool HasKnownResourceForAnyNeed => SelectNeed() != null;

    public NeedState(AnimalBlackboard blackboard, AnimalController controller,
        Func<NeedType, bool> canHandleMissingResource = null)
    {
        _blackboard = blackboard;
        _controller = controller;
        _canHandleMissingResource = canHandleMissingResource;
        Hungry = new HungryState(blackboard, controller);
        _states = new Dictionary<NeedType, BaseState>
        {
            { NeedType.Hunger, Hungry },
            { NeedType.Thirst, new ThirstyState(blackboard, controller) },
            { NeedType.Fatigue, new FatiguedState(blackboard, controller) }
        };
        OnEnter += () =>
        {
            foreach (var need in _blackboard.Needs) need.SatisfactionChanged += OnSatisfactionChanged;
            UpdateCurrentNeed();
        };
        OnUpdate += () => { UpdateCurrentNeed(); _currentState?.InvokeOnUpdate(); };
        OnFixedUpdate += () => _currentState?.InvokeOnFixedUpdate();
        OnExit += () =>
        {
            foreach (var need in _blackboard.Needs) need.SatisfactionChanged -= OnSatisfactionChanged;
            _currentState?.InvokeOnExit();
            _currentState = null;
            CurrentNeed = null;
            _controller.StopMovement();
        };
    }

    private void OnSatisfactionChanged(Need need) => UpdateCurrentNeed();

    public Need SelectNeed()
    {
        var candidates = _blackboard.Needs.Where(need => !need.IsSatisfied)
            .Where(need => _blackboard.HasKnownResource(need.Type, _controller.Animal)
                || (_canHandleMissingResource?.Invoke(need.Type) ?? false))
            .OrderBy(need => need.Type != NeedType.Hunger
                && need.CurrentValue / Mathf.Max(1f, need.MaxValue) < 0.25f ? 0 : 1)
            .ThenBy(need => need.CurrentValue / Mathf.Max(1f, need.MaxValue)).ToList();
        var current = candidates.FirstOrDefault(need => need.Type == CurrentNeed);
        if (current != null && (current.Type != NeedType.Hunger || !candidates.Any(need =>
            need.Type != NeedType.Hunger && need.CurrentValue / Mathf.Max(1f, need.MaxValue) < 0.25f)))
            return current;
        return candidates.FirstOrDefault();
    }

    private void UpdateCurrentNeed()
    {
        var next = SelectNeed();
        if (next?.Type == CurrentNeed) return;
        _currentState?.InvokeOnExit();
        _controller.StopMovement();
        CurrentNeed = next?.Type;
        _currentState = next == null ? null : _states[next.Type];
        _currentState?.InvokeOnEnter();
    }
}
