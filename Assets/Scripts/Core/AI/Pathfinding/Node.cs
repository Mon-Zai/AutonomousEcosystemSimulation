using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding
{
    public class Node : MonoBehaviour
    {
        public Material blockMaterial;
        [SerializeField] List<Node> _neighbors = new List<Node>(); //Serialized for debugging purposes
        [SerializeField] int _cost = 1;
        public bool IsBlock { get; private set; }
        public int Cost => _cost;
        public List<Node> Neighbors => _neighbors;

        public void SetNeighbor(Node newNode)
        {
            _neighbors.Add(newNode);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 6)
            {
                IsBlock = true;
                GetComponent<MeshRenderer>().material = blockMaterial;
            }

            if (other.gameObject.layer == 4)
            {
                _cost = 5;
                GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.3f);
            }
        }

        void OnDrawGizmosSelected()
        {
            if(_neighbors == null || _neighbors.Count == 0) return;
            Gizmos.color = Color.green;
            foreach (var neighbor in _neighbors)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }

    }
}
