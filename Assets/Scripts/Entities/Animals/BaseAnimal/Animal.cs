using System.Collections.Generic;
using UnityEngine;

public abstract class Animal : Entity
{
    [SerializeField] protected List<FoodType> _foodPreferences;
    [SerializeField] protected AnimalSettings _settings;
    public List<FoodType> FoodPreferences => _foodPreferences;
    public AnimalSettings Settings => _settings;
    private Health _healthComponent;

    protected override void Awake()
    {
        base.Awake();
        _healthComponent = GetComponent<Health>();
        if (_healthComponent != null) _healthComponent.OnDeath += Die;
    }

    private void OnDestroy()
    {
        if (_healthComponent != null) _healthComponent.OnDeath -= Die;
    }

    private void Die()
    {
        GetComponent<PathFinder>()?.Stop();
        Stop();
        var controller = GetComponent<AnimalController>();
        if (controller != null) controller.enabled = false;
        var perception = GetComponent<AnimalPerception>();
        if (perception != null) perception.enabled = false;
        var blackboard = GetComponent<AnimalBlackboard>();
        if (blackboard != null && blackboard.Needs != null)
            foreach (var need in blackboard.Needs)
            {
                need.StopAllCoroutines();
                need.enabled = false;
            }
        enabled = false;
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
 
    }
    protected void FixedUpdate()
    {
       base.Update();
        _acceleration = Vector3.ClampMagnitude(_acceleration, _settings.MaxAcceleration);
        Vector3 newVelocity = _velocity + _acceleration * Time.fixedDeltaTime;
        newVelocity = Vector3.ClampMagnitude(newVelocity, _settings.MaxSpeed);
        newVelocity *= 1f - _settings.linearDrag;
        _velocity = newVelocity;
        transform.position += _velocity * Time.fixedDeltaTime;
        _acceleration = Vector3.zero;
    }
}
