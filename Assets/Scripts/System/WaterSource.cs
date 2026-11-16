using UnityEngine;

[RequireComponent(typeof(SpatialMember))]
public class WaterSource : MonoBehaviour
{
    [SerializeField] private float _hydrationValue = 25f;
    private void Awake()
    {
        if (GetComponent<SpatialMember>() == null) gameObject.AddComponent<SpatialMember>();
    }

    public void Drink(Need need)
    {
        if (need != null)
            need.AddToCurrentValue(_hydrationValue);
    }
}
