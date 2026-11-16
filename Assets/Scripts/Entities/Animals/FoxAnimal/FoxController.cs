using UnityEngine;

[RequireComponent(typeof(Fox), typeof(FoxBlackboard), typeof(AnimalPerception))]
[RequireComponent(typeof(PathFinder))]
public class FoxController : AnimalController
{
    [SerializeField] private string _currentState;
    private FoxFSM _foxFSM;
    public Fox Fox => (Fox)_animal;
    public AnimalPerception Perception => _perception;
    public string CurrentState => _currentState;
    public NeedType? CurrentNeed => _foxFSM?.CurrentNeed;
    private void Start()
    {
        var blackboard = GetComponent<FoxBlackboard>();
        if (Fox.Settings == null || Fox.FoodPreferences == null || blackboard.Hunger == null)
        {
            Debug.LogError("Fox requires settings, food preferences and a Hunger need.", this);
            enabled = false;
            return;
        }
        _perception.Initialize(blackboard);
        _foxFSM = new FoxFSM(this, blackboard);
    }
    private void Update() { _foxFSM?.Update(); _currentState = _foxFSM?.CurrentState; }
    private void FixedUpdate() => _foxFSM?.FixedUpdate();
    private void OnDisable() => _foxFSM?.Stop();
    private void OnEnable() => _foxFSM?.Resume();
    public bool PathFailed => _pathFinder.HasPathFailed;
}
