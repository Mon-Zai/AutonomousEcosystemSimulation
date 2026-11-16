using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPerception : MonoBehaviour
{
    [SerializeField] private float _detectionRadius = 8f;
    [SerializeField] private float _scanInterval = 1f;
    [SerializeField] private LayerMask _resourceLayers = ~0;
    private AnimalBlackboard _blackboard;
    private float _nextScanTime;
    private readonly List<SpatialMember> _results = new();
    public IReadOnlyList<SpatialMember> Results => _results;
    public event Action Scanned;
    public void Initialize(AnimalBlackboard blackboard) => _blackboard = blackboard;
    private void OnEnable() => PerceptionScheduler.Register(this);
    private void OnDisable() => PerceptionScheduler.Unregister(this);
    public void ProcessScan()
    {
        if (_blackboard == null || Time.time < _nextScanTime) return;
        _nextScanTime = Time.time + Mathf.Max(0.1f, _scanInterval);
        SpatialGrid.Query(transform.position, _detectionRadius, _results);
        _results.RemoveAll(member => (_resourceLayers.value & (1 << member.gameObject.layer)) == 0);
        _blackboard.ForgetUnavailableResources();
        foreach (var member in _results)
        {
            _blackboard.Remember(member.GetComponent<Food>());
            _blackboard.Remember(member.GetComponent<WaterSource>());
            _blackboard.Remember(member.GetComponent<RestArea>());
        }
        Scanned?.Invoke();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
