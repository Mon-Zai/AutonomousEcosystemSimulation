using System;

public class FoxActionState : BaseState
{
    protected FoxActionState(FoxController controller, Action decide)
    {
        Controller = controller;
        Decide = decide;
    }

    protected FoxController Controller { get; }
    protected Action Decide { get; }

    public FoxActionState(FoxController controller, Action decide, Action update,
        Action enter = null, Action exit = null)
        : this(controller, decide)
    {
        Configure(update, enter, exit);
    }

    protected void Configure(Action update, Action enter = null, Action exit = null)
    {
        OnEnter += () =>
        {
            Controller.Perception.Scanned += Decide;
            enter?.Invoke();
        };
        OnUpdate += update;
        OnExit += () =>
        {
            Controller.Perception.Scanned -= Decide;
            exit?.Invoke();
            Controller.StopMovement();
        };
    }
}
