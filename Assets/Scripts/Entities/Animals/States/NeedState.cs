using System.Collections.Generic;
using System.Linq;

public class NeedState : ParentBaseState
{
    private NeedType? _currentNeed;
    private AnimalBlackboard _animalBlackboard;
    private AnimalController _animalController;
    private Dictionary<NeedType, BaseState> _needStates;
    public bool HasKnownResourceForAnyNeed => _animalBlackboard.Needs != null && _animalBlackboard.Needs
        .Where(need => !need.IsSatisfied)
        .Any(need => _animalBlackboard.HasKnownResource(need.Type, _animalController.Animal));

    public NeedState(AnimalBlackboard animalBlackboard, AnimalController animalController) : base()
    {
        _animalBlackboard = animalBlackboard;
        _animalController = animalController;

        var hungryState = new HungryState(animalBlackboard, animalController);
        var thirstyState = new ThirstyState(animalBlackboard, animalController);
        var fatiguedState = new FatiguedState(animalBlackboard, animalController);

        _needStates = new Dictionary<NeedType, BaseState>
        {
            { NeedType.Hunger, hungryState },
            { NeedType.Thirst, thirstyState },
            { NeedType.Fatigue, fatiguedState }
        };
        foreach (var state in _animalBlackboard.Needs)
        {
            state.SatisfactionChanged += UpdateCurrentNeed;
        }
    }

    public override void OnEnter()
    {
        UpdateCurrentNeed();
    }
    public override void Update()
    {
        _currentState?.Update();
    }

    public override void FixedUpdate() => _currentState?.FixedUpdate();

    private void UpdateCurrentNeed(Need need = null)
    {
        if (need != null && need.Type != _currentNeed) return;
        var next = _animalBlackboard.Needs
            .Where(x => !x.IsSatisfied)
            .Where(x => _animalBlackboard.HasKnownResource(x.Type, _animalController.Animal))
            .OrderBy(x => x.CurrentValue / x.MaxValue)
            .Select(x => (NeedType?)x.Type)
            .FirstOrDefault();

        if (next == _currentNeed && _currentState != null)
            return;
        _currentState?.OnExit();
        _currentNeed = next;

        if (next == null)
        {
            _currentState = null;
            return;
        }

        _currentState = _needStates.TryGetValue(next.Value, out var state) ? state : null;
        _currentState?.OnEnter();
    }
}
