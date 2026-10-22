using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    private class TrackedCustomer
    {
        public CustomerTesting customer;
        public float remainingTime;
        public float initialTime;
    }

    [SerializeField] private Transform _registerPoint;
    [SerializeField] private Transform _waitingPoint;
    [SerializeField] private Vector3 _registerLineDirection = new Vector3(0, 0, -1);
    [SerializeField] private Vector3 _waitingLineDirection = new Vector3(1, 0, 0);
    [SerializeField] private float _registerSpacing = 1.5f;
    [SerializeField] private float _waitingSpacing = 1.5f;

    [SerializeField] private float _extraTimePerItem = 5f;
    [SerializeField] private float _veryHappyThreshold = 0.8f;
    [SerializeField] private float _happyThreshold = 0.6f;
    [SerializeField] private float _neutralThreshold = 0.35f;
    [SerializeField] private float _unhappyThreshold = 0.05f;

    private readonly List<CustomerTesting> _registerQueue = new List<CustomerTesting>();
    private readonly List<CustomerTesting> _waitingCustomers = new List<CustomerTesting>();
    private readonly List<TrackedCustomer> _trackedCustomers = new List<TrackedCustomer>();

    private static CustomerManager _instance;
    public static CustomerManager Instance => _instance;

    public event Action<CustomerTesting, CustomerMood> OnCustomerMoodChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void AddToQueue(CustomerTesting customer)
    {
        if (customer == null || _registerQueue.Contains(customer))
        {
            return;
        }

        _registerQueue.Add(customer);
        UpdateRegisterQueuePositions();
    }

    public bool CanServeAtRegister(CustomerTesting customer)
    {
        return _registerQueue.Count > 0 && _registerQueue[0] == customer;
    }

    public void ServeAtRegister(CustomerTesting customer)
    {
        if (!CanServeAtRegister(customer))
        {
            return;
        }

        _registerQueue.RemoveAt(0);
        UpdateRegisterQueuePositions();

        if (!_waitingCustomers.Contains(customer))
        {
            _waitingCustomers.Add(customer);
        }

        UpdateWaitingPositions();
    }

    public void RegisterWaitingCustomer(CustomerTesting customer, float basePatience)
    {
        if (customer == null)
        {
            return;
        }

        foreach (TrackedCustomer tracked in _trackedCustomers)
        {
            if (tracked.customer == customer)
            {
                return;
            }
        }

        int extraItems = Mathf.Max(0, customer.PendingOrder.Count - 1);
        float totalPatience = basePatience + (_extraTimePerItem * extraItems);

        _trackedCustomers.Add(new TrackedCustomer
        {
            customer = customer,
            remainingTime = totalPatience,
            initialTime = Mathf.Max(0.01f, totalPatience)
        });

        customer.OnOrderCompleted += HandleOrderCompleted;
        customer.OnCustomerLeft += HandleCustomerLeft;
    }

    private void UpdateRegisterQueuePositions()
    {
        Vector3 basePosition = _registerPoint != null ? _registerPoint.position : Vector3.zero;

        for (int i = 0; i < _registerQueue.Count; i++)
        {
            Vector3 targetPosition = basePosition + (_registerLineDirection.normalized * (i * _registerSpacing));
            _registerQueue[i].SetTargetQueuePosition(targetPosition);
        }
    }

    private void UpdateWaitingPositions()
    {
        Vector3 basePosition = _waitingPoint != null ? _waitingPoint.position : Vector3.zero;

        for (int i = 0; i < _waitingCustomers.Count; i++)
        {
            Vector3 targetPosition = basePosition + (_waitingLineDirection.normalized * (i * _waitingSpacing));
            _waitingCustomers[i].SetTargetWaitingPosition(targetPosition);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
        {
            return;
        }

        for (int i = _trackedCustomers.Count - 1; i >= 0; i--)
        {
            TrackedCustomer entry = _trackedCustomers[i];

            if (entry.customer == null)
            {
                _trackedCustomers.RemoveAt(i);
                continue;
            }

            if (entry.customer.State != CustomerTesting.CustomerState.WaitingForOrder)
            {
                continue;
            }

            entry.remainingTime -= Time.deltaTime;

            float ratio = Mathf.Clamp01(entry.remainingTime / entry.initialTime);
            CustomerMood currentMood = GetCustomerMood(ratio);

            OnCustomerMoodChanged?.Invoke(entry.customer, currentMood);

            if (entry.remainingTime <= 0f)
            {
                entry.customer.Leave(false);
            }
        }
    }

    private CustomerMood GetCustomerMood(float ratio)
    {
        if (ReputationManager.Instance != null)
        {
            return ReputationManager.Instance.CalculateCustomerMood(ratio);
        }

        if (ratio >= _veryHappyThreshold)
        {
            return CustomerMood.VeryHappy;
        }
        if (ratio >= _happyThreshold)
        {
            return CustomerMood.Happy;
        }
        if (ratio >= _neutralThreshold)
        {
            return CustomerMood.Neutral;
        }
        if (ratio >= _unhappyThreshold)
        {
            return CustomerMood.Unhappy;
        }
        return CustomerMood.Angry;
    }

    private void HandleOrderCompleted(CustomerTesting customer)
    {
        RemoveTrackedCustomer(customer);
    }

    private void HandleCustomerLeft(CustomerTesting customer, bool satisfied)
    {
        RemoveTrackedCustomer(customer);
    }

    private void RemoveTrackedCustomer(CustomerTesting customer)
    {
        if (customer == null)
        {
            return;
        }

        customer.OnOrderCompleted -= HandleOrderCompleted;
        customer.OnCustomerLeft -= HandleCustomerLeft;

        _registerQueue.Remove(customer);
        _waitingCustomers.Remove(customer);

        UpdateRegisterQueuePositions();
        UpdateWaitingPositions();

        for (int i = _trackedCustomers.Count - 1; i >= 0; i--)
        {
            if (_trackedCustomers[i].customer == customer)
            {
                _trackedCustomers.RemoveAt(i);
            }
        }
    }
}