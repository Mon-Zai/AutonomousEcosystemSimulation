using System;

public class State
{
    public event Action OnEnter;
    public event Action OnUpdate;
    public event Action OnFixedUpdate;
    public event Action OnExit;

    public void InvokeOnUpdate() => OnUpdate?.Invoke();
    public void InvokeOnFixedUpdate() => OnFixedUpdate?.Invoke();
    public void InvokeOnEnter() => OnEnter?.Invoke();
    public void InvokeOnExit() => OnExit?.Invoke();
}