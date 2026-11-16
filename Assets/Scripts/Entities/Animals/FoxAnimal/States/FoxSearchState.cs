using System;
using UnityEngine;

public class FoxSearchState : FoxActionState
{
    private readonly WanderState _wander;
    private float _searchUntil;

    public FoxSearchState(FoxController controller, Action decide, WanderState wander)
        : base(controller, decide)
    {
        _wander = wander;
        Configure(Search, BeginSearch, _wander.InvokeOnExit);
    }

    private void BeginSearch()
    {
        _searchUntil = Time.time + 15f;
        _wander.InvokeOnEnter();
    }

    private void Search()
    {
        _wander.InvokeOnUpdate();
        if (Time.time >= _searchUntil)
        {
            Controller.StopMovement();
            _searchUntil = Time.time + 15f;
            _wander.InvokeOnEnter();
        }
    }
}
