using UnityEngine;

[RequireComponent(typeof(Health), typeof(SpatialMember))]
public class Prey : MonoBehaviour
{
    [SerializeField] private FoodType _foodType = FoodType.Meat;
    [SerializeField] private int _nutritionValue = 60;
    private Health _health;
    public FoodType FoodType => _foodType;
    public int NutritionValue => _nutritionValue;
    public Health Health => _health;
    public bool IsAvailable => isActiveAndEnabled && _health != null && !_health.IsDead;
    private void Awake() => _health = GetComponent<Health>();
    private void OnEnable() => _health.OnDeath += BecomeFood;
    private void OnDisable() => _health.OnDeath -= BecomeFood;
    private void BecomeFood()
    {
        var food = GetComponent<Food>();
        if (food == null) food = gameObject.AddComponent<Food>();
        food.Initialize(_foodType, _nutritionValue);
        enabled = false;
    }
}
