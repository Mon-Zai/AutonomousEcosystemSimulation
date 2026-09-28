using UnityEngine;

public abstract class AnimalController : MonoBehaviour
{
    protected Animal _animal;
    protected PathFinder _pathFinder;
    protected AnimalPerception _perception;

    public Animal Animal => _animal;

    public NodeManager NodeManager => _pathFinder != null ? _pathFinder.NodeManager : null;

    public bool HasReachedDestination => _pathFinder != null && _pathFinder.HasReachedDestination;

    protected virtual void Awake()
    {
        _animal = GetComponent<Animal>();
        _pathFinder = GetComponent<PathFinder>();
        _perception = GetComponent<AnimalPerception>();
    }

    public void MoveTo(Vector3 targetPosition)
    {
        _pathFinder?.MoveTo(targetPosition);
    }
}
