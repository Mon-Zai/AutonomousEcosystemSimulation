using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected float health;
    protected Vector3 _acceleration;
    protected Vector3 _velocity = Vector3.zero;
    public Vector3 Velocity { get { return _velocity; } private set { _velocity = value; } }
    protected virtual void Awake()
    {
    }
    protected virtual void Update()
    {
        Vector3 flatVelocity = new Vector3(_velocity.x, 0f, _velocity.z);
        if (flatVelocity.sqrMagnitude > 0.001f)
            transform.forward = flatVelocity.normalized;
    }

    public float Health { get { return health; } protected set { health = value; } }
    public virtual void ApplyForce(Vector3 force)
    {
        force = new Vector3(force.x, 0f, force.z);
        _acceleration += force;
    }

}
