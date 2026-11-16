using UnityEngine;

[RequireComponent(typeof(Health), typeof(SpatialMember))]
public class Fox : Animal
{
    [SerializeField] private float _attackDamage = 25f;
    [SerializeField] private float _attackInterval = 0.8f;
    [SerializeField] private float _attackRange = 1.2f;
    public float AttackDamage => _attackDamage;
    public float AttackInterval => Mathf.Max(0.1f, _attackInterval);
    public float AttackRange => Mathf.Max(0.1f, _attackRange);
}
