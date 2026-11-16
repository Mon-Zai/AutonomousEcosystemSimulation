using UnityEngine;
public enum FoodType
{
    Fruit,
    Vegetable,
    Meat,
    Grain
}
[RequireComponent(typeof(SpatialMember))]
public class Food : MonoBehaviour, IFood
{
    [SerializeField] protected int _nutritionValue;
    [SerializeField] protected FoodType _foodType;
    public int NutritionValue => _nutritionValue;
    private bool _consumed;
    private void Awake()
    {
        if (GetComponent<SpatialMember>() == null) gameObject.AddComponent<SpatialMember>();
    }
    public void Initialize(FoodType foodType, int nutritionValue)
    {
        _foodType = foodType;
        _nutritionValue = nutritionValue;
    }
    public bool TryConsume()
    {
        if (_consumed || !isActiveAndEnabled) return false;
        _consumed = true;
        gameObject.SetActive(false);
        Destroy(gameObject);
        return true;
    }
    public FoodType GetFoodType()
    {
        return _foodType;
    }
    public void Consume()
    {
        TryConsume();
    }
}
