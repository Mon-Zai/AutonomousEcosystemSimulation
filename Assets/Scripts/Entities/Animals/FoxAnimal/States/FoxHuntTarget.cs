using UnityEngine;

internal static class FoxHuntTarget
{
    public static bool CanAttack(FoxController controller, FoxBlackboard blackboard, Component target)
    {
        return target != null &&
            Vector3.Distance(controller.transform.position, target.transform.position) <= controller.Fox.AttackRange &&
            RaycastUtils.IsInLineOfSight(controller.transform.position, target.transform.position, blackboard.ObstacleLayer);
    }
}
