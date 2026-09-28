public enum RabbitState
{
    Wander,
    Need
}
public class RabbitFSM
{
    private StateMachine<RabbitState> _stateMachine;
    private RabbitBlackboard _blackboard;
    private RabbitController _controller;

    private WanderState _wanderState;
    private NeedState _needState;
    public string CurrentState => _stateMachine.CurrentState.ToString();
    public RabbitFSM(RabbitController rabbitController, RabbitBlackboard rabbitBlackboard)
    {
        _controller = rabbitController;
        _blackboard = rabbitBlackboard;
        _stateMachine = new StateMachine<RabbitState>();

        InitializeStates();
        BuildStateTransitions();

        _stateMachine.ChangeState(RabbitState.Wander);
    }

    public void Update() => _stateMachine.Update();

    public void FixedUpdate() => _stateMachine.FixedUpdate();

    private void InitializeStates()
    {
        _wanderState = new WanderState(_controller);
        _needState = new NeedState(_blackboard, _controller);
    }
    private void BuildStateTransitions()
    {
        IPredicate hasActionableNeeds = new AndPredicate(
            () => _blackboard.HasNeeds && !_blackboard.NeedsSatisfied,
            () => _needState.HasKnownResourceForAnyNeed);
        IPredicate hasNoActionableNeeds = new InvertPredicate(hasActionableNeeds);

        _stateMachine.AddState(RabbitState.Wander, _wanderState)
        .TransitionTo(RabbitState.Need, hasActionableNeeds);

        _stateMachine.AddState(RabbitState.Need, _needState)
        .TransitionTo(RabbitState.Wander, hasNoActionableNeeds);
    }
}
