
/// <summary>
/// Base state class for all parent states.
/// T represents the type of substates the state machine manages.
/// </summary>
public abstract class ParentBaseState : BaseState
{
    protected State _currentState;

    public ParentBaseState()
    {
        
    }
}