using System.Linq;
using AI.Pathfinding;
using UnityEngine;

public class WanderState : BaseState
{
    private AnimalController _animalController;
    private Node _targetNode;
    private const float MinimumTargetDistance = 1f;

    public WanderState(AnimalController animalController)
    {
        _animalController = animalController;
        OnEnter += SelectNextDestination;
        OnUpdate += TryWander;
        OnExit += ResetDestination;
    }

    private void TryWander()
    {
        _animalController.Animal.PerformWanderActivity();

        if (_targetNode == null)
        {
            SelectNextDestination();
            return;
        }

        _animalController.MoveTo(_targetNode.transform.position);
        if (_animalController.HasReachedDestination)
            SelectNextDestination();
    }

    private void ResetDestination()
    {
        _targetNode = null;
    }

    private void SelectNextDestination()
    {
        var nodeManager = _animalController.NodeManager;
        if (nodeManager == null)
        {
            _targetNode = null;
            return;
        }

        float minimumDistanceSquared = MinimumTargetDistance * MinimumTargetDistance;
        _targetNode = nodeManager.Nodes
            .Where(node => node != null && !node.IsBlock)
            .Where(node => (node.transform.position - _animalController.Animal.transform.position).sqrMagnitude > minimumDistanceSquared)
            .OrderBy(node => Random.value)
            .FirstOrDefault();
    }
}
