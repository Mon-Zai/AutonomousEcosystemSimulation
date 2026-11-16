using System;
using UnityEngine;

public class FoxAttackState : FoxActionState
{
    private readonly FoxBlackboard _blackboard;
    private readonly Action<FoxState> _trigger;
    private float _nextAttackTime;

    public Prey AttackedPrey { get; private set; }

    public FoxAttackState(FoxController controller, FoxBlackboard blackboard, Action decide, Action<FoxState> trigger)
        : base(controller, decide)
    {
        _blackboard = blackboard;
        _trigger = trigger;
        Configure(Attack, BeginAttack, EndAttack);
    }

    private void BeginAttack()
    {
        Controller.StopMovement();
        AttackedPrey = _blackboard.PreyTarget;
        if (AttackedPrey != null) AttackedPrey.Health.OnDeath += OnPreyDeath;
    }

    private void EndAttack()
    {
        if (AttackedPrey != null) AttackedPrey.Health.OnDeath -= OnPreyDeath;
        AttackedPrey = null;
    }

    private void OnPreyDeath()
    {
        if (AttackedPrey != null) _blackboard.Remember(AttackedPrey.GetComponent<Food>());
        _trigger(FoxState.Need);
    }

    private void Attack()
    {
        if (AttackedPrey == null || !AttackedPrey.IsAvailable) { Decide(); return; }
        if (!FoxHuntTarget.CanAttack(Controller, _blackboard, AttackedPrey)) { _trigger(FoxState.Chase); return; }
        Controller.StopMovement();
        if (Time.time < _nextAttackTime) return;
        _nextAttackTime = Time.time + Controller.Fox.AttackInterval;
        AttackedPrey.Health.ApplyDamage(Controller.Fox.AttackDamage);
    }
}
