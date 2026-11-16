using System;

public class FoxChaseState : FoxActionState
{
    private readonly FoxBlackboard _blackboard;
    private readonly Action<FoxState> _trigger;

    public FoxChaseState(FoxController controller, FoxBlackboard blackboard, Action decide, Action<FoxState> trigger)
        : base(controller, decide)
    {
        _blackboard = blackboard;
        _trigger = trigger;
        Configure(Chase);
    }

    private void Chase()
    {
        var prey = _blackboard.PreyTarget;
        if (prey == null || !prey.IsAvailable) { Decide(); return; }
        if (FoxHuntTarget.CanAttack(Controller, _blackboard, prey)) { _trigger(FoxState.Attack); return; }
        Controller.MoveTo(prey.transform.position);
    }
}
