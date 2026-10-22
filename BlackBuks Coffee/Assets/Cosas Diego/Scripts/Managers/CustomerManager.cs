
using System;
using System.Collections.Generic;
using UnityEngine;

public enum CustomerMood
{
    Happy,
    Impatient,
    Angry
}

public class CustomerManager : MonoBehaviour
{
    private class TrackedCustomer
    {
        public CustomerTesting customer;
        public float remainingTime;
        public float initialTime;
    }

    [Header("Puntos de atención")]
    [SerializeField] private Transform registerPoint;
    [SerializeField] private Transform waitingPoint;
    [SerializeField] private Vector3 registerLineDirection = new Vector3(0, 0, -1);
    [SerializeField] private Vector3 waitingLineDirection = new Vector3(1, 0, 0);
    [SerializeField] private float registerSpacing = 1.5f;
    [SerializeField] private float waitingSpacing = 1.5f;

    [Header("Paciencia")]
    [SerializeField] private float _extraTimePerItem = 5f;
    [SerializeField] private float _impatientThreshold = 0.5f;
    [SerializeField] private float _angryThreshold = 0.2f;

    private readonly List<CustomerTesting> _registerQueue =
        new List<CustomerTesting>();

    private readonly List<CustomerTesting> _waitingCustomers =
        new List<CustomerTesting>();

    private readonly List<TrackedCustomer> _trackedCustomers =
        new List<TrackedCustomer>();

    public static CustomerManager Instance { get; private set; }

    public event Action<CustomerTesting, CustomerMood> OnCustomerMoodChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void AddToQueue(CustomerTesting customer)
    {
        if (customer == null || _registerQueue.Contains(customer))
            return;

        _registerQueue.Add(customer);
        UpdateRegisterQueuePositions();
    }

    public bool CanServeAtRegister(CustomerTesting customer)
    {
        return _registerQueue.Count > 0
            && _registerQueue[0] == customer;
    }

    public void ServeAtRegister(CustomerTesting customer)
    {
        if (!CanServeAtRegister(customer))
            return;

        _registerQueue.RemoveAt(0);
        UpdateRegisterQueuePositions();

        if (!_waitingCustomers.Contains(customer))
            _waitingCustomers.Add(customer);

        UpdateWaitingPositions();
    }

    public CustomerTesting FindWaitingCustomer(string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            return null;

        string searchedName = customerName.Trim();

        foreach (CustomerTesting customer in _waitingCustomers)
        {
            if (customer == null)
                continue;

            if (customer.State != CustomerTesting.CustomerState.WaitingForOrder)
                continue;

            if (string.Equals(
                customer.CustomerName.Trim(),
                searchedName,
                StringComparison.OrdinalIgnoreCase))
            {
                return customer;
            }
        }

        return null;
    }

    public void RemoveFromWaitingQueue(CustomerTesting customer)
    {
        if (customer == null)
            return;

        if (_waitingCustomers.Remove(customer))
            UpdateWaitingPositions();
    }

    public void RegisterWaitingCustomer(
        CustomerTesting customer,
        float basePatience)
    {
        if (customer == null)
            return;

        foreach (TrackedCustomer tracked in _trackedCustomers)
        {
            if (tracked.customer == customer)
                return;
        }

        int extraItems = Mathf.Max(0, customer.PendingOrder.Count - 1);
        float totalPatience = basePatience + _extraTimePerItem * extraItems;

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
        Vector3 basePosition = registerPoint != null
            ? registerPoint.position
            : Vector3.zero;

        for (int i = 0; i < _registerQueue.Count; i++)
        {
            Vector3 targetPosition = basePosition
                + registerLineDirection.normalized * (i * registerSpacing);

            _registerQueue[i].SetTargetQueuePosition(targetPosition);
        }
    }

    private void UpdateWaitingPositions()
    {
        Vector3 basePosition = waitingPoint != null
            ? waitingPoint.position
            : Vector3.zero;

        for (int i = 0; i < _waitingCustomers.Count; i++)
        {
            Vector3 targetPosition = basePosition
                + waitingLineDirection.normalized * (i * waitingSpacing);

            _waitingCustomers[i].SetTargetWaitingPosition(targetPosition);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            return;

        for (int i = _trackedCustomers.Count - 1; i >= 0; i--)
        {
            TrackedCustomer entry = _trackedCustomers[i];

            if (entry.customer == null)
            {
                _trackedCustomers.RemoveAt(i);
                continue;
            }

            if (entry.customer.State != CustomerTesting.CustomerState.WaitingForOrder)
                continue;

            entry.remainingTime -= Time.deltaTime;

            float ratio = entry.remainingTime / entry.initialTime;
            OnCustomerMoodChanged?.Invoke(entry.customer, GetMood(ratio));

            if (entry.remainingTime <= 0f)
                entry.customer.Leave(false);
        }
    }

    private CustomerMood GetMood(float ratio)
    {
        if (ratio <= _angryThreshold)
            return CustomerMood.Angry;

        if (ratio <= _impatientThreshold)
            return CustomerMood.Impatient;

        return CustomerMood.Happy;
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
            return;

        customer.OnOrderCompleted -= HandleOrderCompleted;
        customer.OnCustomerLeft -= HandleCustomerLeft;

        _registerQueue.Remove(customer);
        _waitingCustomers.Remove(customer);

        UpdateRegisterQueuePositions();
        UpdateWaitingPositions();

        for (int i = _trackedCustomers.Count - 1; i >= 0; i--)
        {
            if (_trackedCustomers[i].customer == customer)
                _trackedCustomers.RemoveAt(i);
        }
    }
}
