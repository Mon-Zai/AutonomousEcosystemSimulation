using System.Collections;
using System.Collections.Generic;
using AI.Pathfinding;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private NodeManager _nodeManager;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private AISettings _settings;
    [SerializeField] private float _minDisntanceToTarget = 0.5f;
    [SerializeField] private Entity _entity;

    private bool _hasReachedDestination = false;
    private bool _isFollowingPath = false;

    private SteeringBehavior _steeringBehavior;

    public bool HasReachedDestination => _hasReachedDestination;

    void Awake()
    {
        _entity = GetComponent<Entity>();
        _steeringBehavior = new SteeringBehavior(_entity);
        _nodeManager = FindFirstObjectByType<NodeManager>();
    }

    public NodeManager NodeManager => _nodeManager;

    public void MoveTo(Vector3 targetPosition, bool forcePathfinding = false)
    {
        Vector3 toTarget = targetPosition - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude <= _minDisntanceToTarget * _minDisntanceToTarget)
        {
            _hasReachedDestination = true;
            _isFollowingPath = false;
            return;
        }

        _hasReachedDestination = false;

        if (RaycastUtils.IsInLineOfSight(transform.position, targetPosition, _obstacleLayer) && !forcePathfinding)
        {
            _isFollowingPath = false; 
            Vector3 steeringForce = _steeringBehavior.Seek(targetPosition, _settings.MaxSpeed);
            _entity.ApplyForce(steeringForce);
            Debug.Log("Directly seeking target: " + targetPosition);
            return;
        }
        Node startNode = _nodeManager.GetNode(transform.position);
        Node targetNode = _nodeManager.GetNode(targetPosition);

        if (startNode == null || targetNode == null)
        {
            Debug.LogWarning("Start or target node is null.");
            return;
        }
        if (!_isFollowingPath)
        {
            //Debug.Log("Starting to follow path from " + startNode.transform.position + " to " + targetNode.transform.position);
            StartCoroutine(FollowPath(startNode, targetNode));
        }

    }

    IEnumerator FollowPath(Node start, Node end)
    {
        _isFollowingPath = true;
        List<Node> path = Pathfinding.CalculateAStar(start, end);

        while (path.Count > 0)
        {
            if (_isFollowingPath == false)
            {
                path.Clear();
                yield break;
            }
            var dir = path[0].transform.position - transform.position;
            Vector3 steeringForce = _steeringBehavior.Seek(path[0].transform.position, _settings.MaxSpeed);
            _entity.ApplyForce(steeringForce);
            if (dir.magnitude <= _minDisntanceToTarget)
            {
                path.RemoveAt(0);
            }
            yield return null;
        }

        _isFollowingPath = false;
        _hasReachedDestination = true;
    }
}
