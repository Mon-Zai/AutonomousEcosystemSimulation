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
    [SerializeField] private float _pathRetryDelay = 0.5f;
    [SerializeField] private Entity _entity;

    private bool _hasReachedDestination = false;
    private bool _isFollowingPath = false;
    private bool _hasCurrentTarget;
    private bool _pathFailed;
    private Vector3 _currentTarget;
    private float _nextPathRetryTime;
    private Coroutine _followPathCoroutine;

    private SteeringBehavior _steeringBehavior;

    public bool HasReachedDestination => _hasReachedDestination;
    public bool HasPathFailed => _pathFailed;

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
            CancelFollowingPath();
            _currentTarget = targetPosition;
            _hasCurrentTarget = true;
            _pathFailed = false;
            _hasReachedDestination = true;
            return;
        }

        bool targetChanged = !_hasCurrentTarget ||
            (targetPosition - _currentTarget).sqrMagnitude > 0.01f;
        if (targetChanged)
        {
            CancelFollowingPath();
            _currentTarget = targetPosition;
            _hasCurrentTarget = true;
            _pathFailed = false;
            _hasReachedDestination = false;
        }
        else if (_hasReachedDestination)
        {
            return;
        }
        else if (_pathFailed && Time.time < _nextPathRetryTime)
        {
            return;
        }

        _hasReachedDestination = false;

        if (RaycastUtils.IsInLineOfSight(transform.position, targetPosition, _obstacleLayer) && !forcePathfinding)
        {
            CancelFollowingPath();
            _pathFailed = false;
            Vector3 steeringForce = _steeringBehavior.Seek(targetPosition, _settings.MaxSpeed);
            _entity.ApplyForce(steeringForce);
            //Debug.Log("Directly seeking target: " + targetPosition);
            return;
        }

        if (_nodeManager == null)
        {
            MarkPathFailed("NodeManager is missing.");
            return;
        }

        Node startNode = _nodeManager.GetNode(transform.position);
        Node targetNode = _nodeManager.GetNode(targetPosition);

        if (startNode == null || targetNode == null)
        {
            MarkPathFailed("Start or target node is null.");
            return;
        }
        if (!_isFollowingPath)
        {
            //Debug.Log("Starting to follow path from " + startNode.transform.position + " to " + targetNode.transform.position);
            _followPathCoroutine = StartCoroutine(FollowPath(startNode, targetNode));
        }

    }

    private void CancelFollowingPath()
    {
        if (_followPathCoroutine != null)
        {
            StopCoroutine(_followPathCoroutine);
            _followPathCoroutine = null;
        }

        _isFollowingPath = false;
    }

    private void MarkPathFailed(string reason)
    {
        _pathFailed = true;
        _hasReachedDestination = false;
        _isFollowingPath = false;
        _nextPathRetryTime = Time.time + Mathf.Max(0.1f, _pathRetryDelay);
        Debug.LogWarning($"Pathfinding failed: {reason}", this);
    }

    IEnumerator FollowPath(Node start, Node end)
    {
        _isFollowingPath = true;
        List<Node> path = Pathfinding.CalculateAStar(start, end);

        if (path.Count == 0)
        {
            _followPathCoroutine = null;
            MarkPathFailed($"No route from {start.name} to {end.name}.");
            yield break;
        }

        _pathFailed = false;

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
        _followPathCoroutine = null;
        _hasReachedDestination = true;
    }
}
