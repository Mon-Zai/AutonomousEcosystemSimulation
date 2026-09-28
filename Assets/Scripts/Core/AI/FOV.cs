using UnityEngine;

public class FOV : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;
    private float _radius;
    private float _angle;
    public bool IsInFieldOfView(float radius, float angle)
    {
        _radius = radius;
        _angle = angle;
        
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if(rangeChecks.Length == 0)
        {
            return false;
        }
        foreach (Collider col in rangeChecks)
        {
            Transform target = col.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) > angle / 2) return false;
            if (!RaycastUtils.IsInLineOfSight(transform.position, target.position, obstructionMask)) return false;

            break;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
        Vector3 fovLine1 = DirectionFromAngle(transform.eulerAngles.y, -_angle / 2);
        Vector3 fovLine2 = DirectionFromAngle(transform.eulerAngles.y, _angle / 2);
        Gizmos.DrawLine(transform.position, transform.position + fovLine1 * _radius);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2 * _radius);
    }
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}