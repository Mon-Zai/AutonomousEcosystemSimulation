using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDead => _currentHealth <= 0f;
    public event Action<float> OnDamaged;
    public event Action OnDeath;

    private void Awake() => _currentHealth = Mathf.Max(1f, _maxHealth);

    public void ApplyDamage(float damage)
    {
        if (IsDead || damage <= 0f || float.IsNaN(damage)) return;
        float appliedDamage = Mathf.Min(damage, _currentHealth);
        _currentHealth -= appliedDamage;
        bool died = IsDead;
        OnDamaged?.Invoke(appliedDamage);
        if (died) OnDeath?.Invoke();
    }
}
