using System;
using UnityEngine;

public class ReputationManager : MonoBehaviour
{
    private static ReputationManager _instance;
    public static ReputationManager Instance => _instance;

    [SerializeField] private float _maxReputation = 100f;
    private float _currentReputation;

    public event Action<float, float> OnReputationChanged;
    public event Action OnReputationDepleted;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _currentReputation = _maxReputation * 0.5f;
        OnReputationChanged?.Invoke(_currentReputation, _maxReputation);
    }

    public void ProcessCustomerLeftAngry()
    {
        ModifyReputation(-20f);
    }

    private void ModifyReputation(float amount)
    {
        _currentReputation = Mathf.Clamp(_currentReputation + amount, 0f, _maxReputation);
        OnReputationChanged?.Invoke(_currentReputation, _maxReputation);

        if (_currentReputation <= 0f)
        {
            OnReputationDepleted?.Invoke();
        }
    }
}