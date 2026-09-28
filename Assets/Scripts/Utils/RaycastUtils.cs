using UnityEngine;

public static class RaycastUtils
{
    public static bool IsInLineOfSight(Vector3 origin, Vector3 target, LayerMask obstacleMask)
    {
        Vector3 direction = target - origin;
        float distance = direction.magnitude;
        return !Physics.Raycast(origin, direction, distance, obstacleMask);
    }
}
