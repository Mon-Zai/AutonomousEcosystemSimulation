using UnityEngine;

public abstract class AISettings : ScriptableObject
{
    [Header("General")]
    public float MaxSpeed = 5f;
    public float MaxAcceleration = 0.5f;
    public float SlowingRadius = 2f;
    public float linearDrag = 0.01f;
    [Header("Prediction")]
    public float PursuePredictionTime = 1f;
    public float EvadePredictionTime = 1f;
}
