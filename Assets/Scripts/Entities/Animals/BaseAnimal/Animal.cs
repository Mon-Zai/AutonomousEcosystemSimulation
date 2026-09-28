using System.Collections.Generic;
using UnityEngine;

public abstract class Animal : Entity
{
    [SerializeField] protected List<FoodType> _foodPreferences;
    [SerializeField] protected AnimalSettings _settings;
    public List<FoodType> FoodPreferences => _foodPreferences;
    public AnimalSettings Settings => _settings;

    protected override void Awake()
    {
        base.Awake();
    }

    public virtual void PerformWanderActivity()
    {
    }

    public void Stop()
    {
        _velocity = Vector3.zero;
        _acceleration = Vector3.zero;
    }

    protected override void Update()
    {
        base.Update();
        _acceleration = Vector3.ClampMagnitude(_acceleration, _settings.MaxAcceleration);
        Vector3 newVelocity = _velocity + _acceleration * Time.deltaTime;
        newVelocity = Vector3.ClampMagnitude(newVelocity, _settings.MaxSpeed);
        newVelocity *= 1f - _settings.linearDrag;
        _velocity = newVelocity;
        transform.position += _velocity * Time.deltaTime;
        _acceleration = Vector3.zero;
    }
}
