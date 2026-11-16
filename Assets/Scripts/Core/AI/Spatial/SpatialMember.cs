using System;
using UnityEngine;

[DisallowMultipleComponent]
public class SpatialMember : MonoBehaviour
{
    private Vector3 _lastPosition;
    public event Action<SpatialMember> Moved;

    private void OnEnable()
    {
        _lastPosition = transform.position;
        SpatialGrid.Register(this);
    }

    private void OnDisable() => SpatialGrid.Remove(this);

    private void LateUpdate()
    {
        var position = transform.position;
        if (position.Equals(_lastPosition)) return;
        _lastPosition = position;
        Moved?.Invoke(this);
    }
    private void OnDrawGizmosSelected()
    {
        var cell = SpatialGrid.GetCell(transform.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3((cell.x + 0.5f) * SpatialGrid.CellSize, transform.position.y,
            (cell.y + 0.5f) * SpatialGrid.CellSize), new Vector3(SpatialGrid.CellSize, 0.1f, SpatialGrid.CellSize));
    }
}
