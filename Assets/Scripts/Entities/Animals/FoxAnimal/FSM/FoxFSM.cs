using UnityEngine;

public enum FoxState { Wander, Need, Search, Chase, Attack }

public class FoxFSM
{
    private readonly StateMachine<FoxState> _stateMachine = new();
    private readonly FoxController _controller;
    private readonly FoxBlackboard _blackboard;
    private FoxActionState _wanderState;
    private NeedState _needState;
    private FoxSearchState _searchState;
    private FoxChaseState _chaseState;
    private FoxAttackState _attackState;
    private float _nextDecisionTime;
    private bool _running = true;

    public NeedType? CurrentNeed => _needState.CurrentNeed;
    public string CurrentState => _stateMachine.CurrentKey.ToString();

    public FoxFSM(FoxController controller, FoxBlackboard blackboard)
    {
        _controller = controller;
        _blackboard = blackboard;

        InitializeStates();
        BuildStateTransitions();
        _stateMachine.ChangeState(FoxState.Wander);
    }

    private void InitializeStates()
    {
        var wander = new WanderState(_controller);
        _wanderState = new FoxActionState(_controller, Decide,
            wander.InvokeOnUpdate, wander.InvokeOnEnter, wander.InvokeOnExit);
        _needState = new NeedState(_blackboard, _controller, type => type == NeedType.Hunger);
        _needState.Hungry.OnEnter += () => _needState.Hungry.FoodUnavailable += BeginHunt;
        _needState.Hungry.OnExit += () => _needState.Hungry.FoodUnavailable -= BeginHunt;
        _searchState = new FoxSearchState(_controller, Decide, wander);
        _chaseState = new FoxChaseState(_controller, _blackboard, Decide, Trigger);
        _attackState = new FoxAttackState(_controller, _blackboard, Decide, Trigger);
    }

    private void BuildStateTransitions()
    {
        IPredicate always = new SimplePredicate(() => true);

        _stateMachine.AddState(FoxState.Wander, _wanderState)
            .TriggeredTransitionTo(FoxState.Need, nameof(FoxState.Need), always);

        _stateMachine.AddState(FoxState.Need, _needState)
            .TriggeredTransitionTo(FoxState.Wander, nameof(FoxState.Wander), always)
            .TriggeredTransitionTo(FoxState.Search, nameof(FoxState.Search), always)
            .TriggeredTransitionTo(FoxState.Chase, nameof(FoxState.Chase), always)
            .TriggeredTransitionTo(FoxState.Attack, nameof(FoxState.Attack), always);

        _stateMachine.AddState(FoxState.Search, _searchState)
            .TriggeredTransitionTo(FoxState.Wander, nameof(FoxState.Wander), always)
            .TriggeredTransitionTo(FoxState.Need, nameof(FoxState.Need), always)
            .TriggeredTransitionTo(FoxState.Chase, nameof(FoxState.Chase), always)
            .TriggeredTransitionTo(FoxState.Attack, nameof(FoxState.Attack), always);

        _stateMachine.AddState(FoxState.Chase, _chaseState)
            .TriggeredTransitionTo(FoxState.Wander, nameof(FoxState.Wander), always)
            .TriggeredTransitionTo(FoxState.Need, nameof(FoxState.Need), always)
            .TriggeredTransitionTo(FoxState.Search, nameof(FoxState.Search), always)
            .TriggeredTransitionTo(FoxState.Attack, nameof(FoxState.Attack), always);

        _stateMachine.AddState(FoxState.Attack, _attackState)
            .TriggeredTransitionTo(FoxState.Wander, nameof(FoxState.Wander), always)
            .TriggeredTransitionTo(FoxState.Need, nameof(FoxState.Need), always)
            .TriggeredTransitionTo(FoxState.Search, nameof(FoxState.Search), always)
            .TriggeredTransitionTo(FoxState.Chase, nameof(FoxState.Chase), always);
    }

    private void Trigger(FoxState state) => _stateMachine.Trigger(state.ToString());

    public void Update()
    {
        if (!_running) return;
        if (Time.time >= _nextDecisionTime)
        {
            _nextDecisionTime = Time.time + 0.5f;
            Decide();
        }
        _stateMachine.Update();
    }

    public void FixedUpdate() { if (_running) _stateMachine.FixedUpdate(); }

    public void Stop()
    {
        if (!_running) return;
        _running = false;
        _stateMachine.CurrentState.InvokeOnExit();
    }

    public void Resume()
    {
        if (_running) return;
        _running = true;
        _stateMachine.CurrentState.InvokeOnEnter();
    }

    private void Decide()
    {
        var need = _needState.SelectNeed();
        if (need == null) { Trigger(FoxState.Wander); return; }
        if (_stateMachine.CurrentKey == FoxState.Need) return;
        if (need.Type != NeedType.Hunger || _stateMachine.CurrentKey == FoxState.Wander)
        {
            Trigger(FoxState.Need);
            return;
        }
        BeginHunt();
    }

    private void BeginHunt()
    {
        _blackboard.SelectTargets(_controller.Fox, _controller.Perception);
        if (_blackboard.FoodTarget != null) { Trigger(FoxState.Need); return; }
        var prey = _blackboard.PreyTarget;
        if (prey == null) { Trigger(FoxState.Search); return; }
        if (_stateMachine.CurrentKey == FoxState.Attack && prey != _attackState.AttackedPrey)
        { Trigger(FoxState.Chase); return; }
        Trigger(FoxHuntTarget.CanAttack(_controller, _blackboard, prey) ? FoxState.Attack : FoxState.Chase);
    }
}
