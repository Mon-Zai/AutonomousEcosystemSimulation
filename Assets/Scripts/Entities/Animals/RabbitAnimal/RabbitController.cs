using UnityEngine;

public class RabbitController : AnimalController
{
    [SerializeField] string _currentState;
    private RabbitFSM _rabbitFSM;
    private RabbitBlackboard _blackboard;
    public string CurrentState => _rabbitFSM?.CurrentState;
   
    protected override void Awake()
    {
        base.Awake();
        _blackboard = GetComponent<RabbitBlackboard>();
        _currentState = "";
    }

    private void Start()
    {
        if (_animal == null || _blackboard == null)
        {
            Debug.LogError("RabbitController requires a Rabbit and RabbitBlackboard on the same GameObject.", this);
            enabled = false;
            return;
        }

        _perception?.Initialize(_blackboard);
        _rabbitFSM = new RabbitFSM(this, _blackboard);
    }

    private void Update()
    {
        _perception?.ScanIfNeeded();
        _rabbitFSM?.Update();
        _currentState = _rabbitFSM?.CurrentState;
    }

    private void FixedUpdate() => _rabbitFSM?.FixedUpdate();
}
