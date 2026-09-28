using System;
using System.Collections;
using UnityEngine;
public enum NeedType
{
    Hunger,
    Thirst,
    Fatigue
}
public class Need : MonoBehaviour
{
    [SerializeField] NeedType _needType;
    [SerializeField] float _satisfactionTime;
    [SerializeField] float _maxValue;
    [SerializeField] float _decreaseRate;
    [SerializeField] string _debugValue;
    private float _currentValue;
    private bool _isSatisfied = false;
    public NeedType Type => _needType;
    public float MaxValue => _maxValue;
    public float CurrentValue => _currentValue;
    public bool IsSatisfied => _isSatisfied;

    public event Action<Need> SatisfactionChanged;

    void Awake()
    {
        _currentValue = _maxValue;
        _isSatisfied = true;
    }

    void Start()
    {
        StartCoroutine(_satisfactionCoroutine());
    }

    void Update()
    {
        _debugValue = _currentValue.ToString("F2");
        if (_isSatisfied) return;
        _currentValue -= _decreaseRate * Time.deltaTime;
        if (_currentValue < 0)
        {
            _currentValue = 0;
        }
    }

    public void AddToCurrentValue(float amount)
    {
        _currentValue += amount;
        if (_currentValue > _maxValue)
        {
            _currentValue = _maxValue;
        }
        if (_currentValue >= _maxValue && !_isSatisfied)
        {
            StartCoroutine(_satisfactionCoroutine());
        }
    }

    private IEnumerator _satisfactionCoroutine()
    {
        SetSatisfied(true);
        yield return new WaitForSeconds(_satisfactionTime);
        SetSatisfied(false);
    }

    private void SetSatisfied(bool value)
    {
        if (_isSatisfied == value) return;
        _isSatisfied = value;
        SatisfactionChanged?.Invoke(this);
    }
}
