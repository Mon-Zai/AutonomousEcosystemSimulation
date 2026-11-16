using System;
using System.Collections.Generic;
using UnityEngine;

public static class SpatialGrid
{
    public const float CellSize = 8f;
    private static readonly Dictionary<SpatialMember, Vector2Int> _lastPositions = new();
    private static HashSet<SpatialMember>[,] _buckets;
    private static int _originX;
    private static int _originZ;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        foreach (var member in _lastPositions.Keys)
            if (member != null) member.Moved -= UpdateEntity;
        _lastPositions.Clear();
        _buckets = null;
        _originX = 0;
        _originZ = 0;
    }

    public static Vector2Int GetCell(Vector3 position) => new Vector2Int(
        Mathf.FloorToInt(position.x / CellSize), Mathf.FloorToInt(position.z / CellSize));

    public static void Register(SpatialMember member)
    {
        if (_lastPositions.ContainsKey(member)) return;
        var cell = GetCell(member.transform.position);
        EnsureContains(cell);
        GetOrCreateBucket(cell).Add(member);
        _lastPositions.Add(member, cell);
        member.Moved += UpdateEntity;
    }

    public static void Remove(SpatialMember member)
    {
        if (!_lastPositions.TryGetValue(member, out var cell)) return;
        member.Moved -= UpdateEntity;
        _buckets[cell.x - _originX, cell.y - _originZ].Remove(member);
        _lastPositions.Remove(member);
    }

    private static void UpdateEntity(SpatialMember member)
    {
        var previous = _lastPositions[member];
        var current = GetCell(member.transform.position);
        if (previous == current) return;

        EnsureContains(current);
        _buckets[previous.x - _originX, previous.y - _originZ].Remove(member);
        GetOrCreateBucket(current).Add(member);
        _lastPositions[member] = current;
    }

    private static HashSet<SpatialMember> GetOrCreateBucket(Vector2Int cell)
    {
        int x = cell.x - _originX;
        int z = cell.y - _originZ;
        if (_buckets[x, z] == null) _buckets[x, z] = new HashSet<SpatialMember>();
        return _buckets[x, z];
    }

    private static void EnsureContains(Vector2Int cell)
    {
        if (_buckets == null)
        {
            _originX = cell.x;
            _originZ = cell.y;
            _buckets = new HashSet<SpatialMember>[1, 1];
            return;
        }

        int oldWidth = _buckets.GetLength(0);
        int oldHeight = _buckets.GetLength(1);
        long minX = _originX;
        long minZ = _originZ;
        long maxX = minX + oldWidth - 1L;
        long maxZ = minZ + oldHeight - 1L;
        if (cell.x >= minX && cell.x <= maxX && cell.y >= minZ && cell.y <= maxZ) return;

        long newMinX = cell.x < minX ? Math.Min((long)cell.x, minX - oldWidth) : minX;
        long newMinZ = cell.y < minZ ? Math.Min((long)cell.y, minZ - oldHeight) : minZ;
        long newMaxX = cell.x > maxX ? Math.Max((long)cell.x, maxX + oldWidth) : maxX;
        long newMaxZ = cell.y > maxZ ? Math.Max((long)cell.y, maxZ + oldHeight) : maxZ;
        long width = newMaxX - newMinX + 1;
        long height = newMaxZ - newMinZ + 1;
        if (newMinX < int.MinValue || newMinZ < int.MinValue ||
            width > int.MaxValue || height > int.MaxValue || width * height > int.MaxValue)
            throw new InvalidOperationException("Spatial grid dimensions exceed the supported array size.");

        var expanded = new HashSet<SpatialMember>[(int)width, (int)height];
        int offsetX = (int)(minX - newMinX);
        int offsetZ = (int)(minZ - newMinZ);
        for (int x = 0; x < oldWidth; x++)
        for (int z = 0; z < oldHeight; z++)
            expanded[x + offsetX, z + offsetZ] = _buckets[x, z];

        _originX = (int)newMinX;
        _originZ = (int)newMinZ;
        _buckets = expanded;
    }

    public static void Query(Vector3 position, float radius, List<SpatialMember> results)
    {
        results.Clear();
        if (_buckets == null) return;

        var minimum = GetCell(position - Vector3.one * radius);
        var maximum = GetCell(position + Vector3.one * radius);
        long minX = Math.Max((long)minimum.x, _originX);
        long minZ = Math.Max((long)minimum.y, _originZ);
        long maxX = Math.Min(maximum.x, (long)_originX + _buckets.GetLength(0) - 1);
        long maxZ = Math.Min(maximum.y, (long)_originZ + _buckets.GetLength(1) - 1);
        for (long x = minX; x <= maxX; x++)
        for (long z = minZ; z <= maxZ; z++)
        {
            var members = _buckets[(int)(x - _originX), (int)(z - _originZ)];
            if (members == null) continue;
            foreach (var member in members)
                if (member != null && member.isActiveAndEnabled &&
                    (member.transform.position - position).sqrMagnitude <= radius * radius)
                    results.Add(member);
        }
    }
}
