using UnityEngine;

public abstract class BaseState : State
{
    
    private void DebugEnter()
    {
        Debug.Log("Entering state: " + GetType().Name);
    }

    private void DebugExit()
    {
        Debug.Log("Exiting state: " + GetType().Name);
    }


}