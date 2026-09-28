using UnityEngine;

public abstract class BaseState : IState
{
    public virtual void FixedUpdate()
    {
    }

    public virtual void OnEnter()
    {
        Debug.Log("Entering state: " + GetType().Name);
    }

    public virtual void OnExit()
    {
        Debug.Log("Exiting state: " + GetType().Name);
    }

    public virtual void Update()
    {
    }
}