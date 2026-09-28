using UnityEngine;
public enum FoodType
{
    Fruit,
    Vegetable,
    Meat,
    Grain
}
public class Food : MonoBehaviour, IFood
{
    [SerializeField] protected int _nutritionValue;
    [SerializeField] protected FoodType _foodType;
    public int NutritionValue => _nutritionValue;
    public FoodType GetFoodType()
    {
        return _foodType;
    }
    public void Consume()
    {
        Destroy(gameObject);
    }
}
