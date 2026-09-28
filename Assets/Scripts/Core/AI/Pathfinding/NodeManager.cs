using System.Collections.Generic;
using UnityEngine;
using AI.Pathfinding;

public class NodeManager : MonoBehaviour
{
    [SerializeField] private LayerMask _obstacleLayer;
    private Node[] _nodes;
    public IEnumerable<Node> Nodes => _nodes;

    void Awake()
    {
        _nodes = GetComponentsInChildren<Node>();
        foreach (var node in _nodes)
        {
            foreach (var otherNode in _nodes)
            {
                if (RaycastUtils.IsInLineOfSight(node.transform.position, otherNode.transform.position, _obstacleLayer))
                {
                    node.SetNeighbor(otherNode);
                }
            }
        }
    }

    public Node GetNode(Vector3 pos)
    {
        Node minNode = null;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < _nodes.Length; i++)
        {
            var dist = Vector3.Distance(_nodes[i].transform.position, pos);

            if (dist < minDist)
            {
                minDist = dist;
                minNode = _nodes[i];
            }
        }

        return minNode;
    }
}
