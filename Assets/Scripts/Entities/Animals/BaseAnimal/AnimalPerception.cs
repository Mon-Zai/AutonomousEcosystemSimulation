using UnityEngine;

public class AnimalPerception : MonoBehaviour
{
    [SerializeField] private float _detectionRadius = 8f;
    [SerializeField] private float _scanInterval = 1f;
    [SerializeField] private LayerMask _resourceLayers = ~0;
    [SerializeField] private int _overlapBufferSize = 64;

    private AnimalBlackboard _blackboard;
    private Collider[] _overlapResults;
    private float _nextScanTime;

    private void Awake()
    {
        _overlapResults = new Collider[Mathf.Max(1, _overlapBufferSize)];
    }

    public void Initialize(AnimalBlackboard blackboard)
    {
        _blackboard = blackboard;
    }

    public void ScanIfNeeded()
    {
        if (_blackboard == null || Time.time < _nextScanTime)
            return;

        _nextScanTime = Time.time + Mathf.Max(0.1f, _scanInterval);
        int overlapCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            _detectionRadius,
            _overlapResults,
            _resourceLayers,
            QueryTriggerInteraction.Collide);

        _blackboard.ForgetUnavailableResources();
        for (int i = 0; i < overlapCount; i++)
        {
            var collider = _overlapResults[i];
            if (collider == null)
                continue;

            _blackboard.Remember(collider.GetComponentInParent<Food>());
            _blackboard.Remember(collider.GetComponentInParent<WaterSource>());
            _blackboard.Remember(collider.GetComponentInParent<RestArea>());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}