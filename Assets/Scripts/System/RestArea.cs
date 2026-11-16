using UnityEngine;

[RequireComponent(typeof(SpatialMember))]
public class RestArea : MonoBehaviour
{
    [SerializeField] private float _energyRecoveryRate = 20f;
    private void Awake()
    {
        if (GetComponent<SpatialMember>() == null) gameObject.AddComponent<SpatialMember>();
    }

    public void Rest(Need need, float deltaTime)
    {
        if (need != null)
            need.AddToCurrentValue(_energyRecoveryRate * deltaTime);
    }
}
