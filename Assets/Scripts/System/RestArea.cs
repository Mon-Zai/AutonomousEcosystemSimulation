using UnityEngine;

public class RestArea : MonoBehaviour
{
    [SerializeField] private float _energyRecoveryRate = 20f;

    public void Rest(Need need, float deltaTime)
    {
        if (need != null)
            need.AddToCurrentValue(_energyRecoveryRate * deltaTime);
    }
}