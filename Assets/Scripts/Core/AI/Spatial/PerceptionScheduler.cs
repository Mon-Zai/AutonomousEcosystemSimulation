using System.Collections.Generic;
using UnityEngine;

public class PerceptionScheduler : MonoBehaviour
{
    [SerializeField, Min(1)] private int _agentsPerFrame = 8;
    private static readonly List<AnimalPerception> _agents = new();
    private static PerceptionScheduler _instance;
    private int _cursor;
    public int Cursor => _cursor;
    public int ProcessedLastFrame { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRegistry() { _agents.Clear(); _instance = null; }
    public static void Register(AnimalPerception agent)
    {
        if (!_agents.Contains(agent)) _agents.Add(agent);
        if (_instance == null)
        {
            var owner = new GameObject("Perception Scheduler");
            _instance = owner.AddComponent<PerceptionScheduler>();
            DontDestroyOnLoad(owner);
        }
    }
    public static void Unregister(AnimalPerception agent) => _agents.Remove(agent);
    private void Update()
    {
        ProcessedLastFrame = 0;
        int count = Mathf.Min(Mathf.Max(1, _agentsPerFrame), _agents.Count);
        for (int i = 0; i < count && _agents.Count > 0; i++)
        {
            _cursor %= _agents.Count;
            var agent = _agents[_cursor++];
            if (agent != null && agent.isActiveAndEnabled) agent.ProcessScan();
            ProcessedLastFrame++;
        }
    }
}
