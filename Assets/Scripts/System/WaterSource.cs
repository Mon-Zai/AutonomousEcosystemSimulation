using UnityEngine;

public class WaterSource : MonoBehaviour
{
    [SerializeField] private float _hydrationValue = 25f;

    public void Drink(Need need)
    {
        if (need != null)
            need.AddToCurrentValue(_hydrationValue);
    }
}