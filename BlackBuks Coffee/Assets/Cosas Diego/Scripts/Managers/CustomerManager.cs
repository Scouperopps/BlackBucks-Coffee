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

    [SerializeField] private float _extraTimePerItem = 5f;
    [SerializeField] private float _impatientThreshold = 0.5f;
    [SerializeField] private float _angryThreshold = 0.2f;

    private readonly List<TrackedCustomer> _trackedCustomers = new List<TrackedCustomer>();

    public static CustomerManager Instance { get; private set; }

    public event Action<CustomerTesting, CustomerMood> OnCustomerMoodChanged;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterWaitingCustomer(CustomerTesting customer, float basePatience)
    {
        int extraItems = Mathf.Max(0, customer.PendingOrder.Count - 1);
        float totalPatience = basePatience + _extraTimePerItem * extraItems;

        TrackedCustomer entry = new TrackedCustomer
        {
            customer = customer,
            remainingTime = totalPatience,
            initialTime = totalPatience
        };

        _trackedCustomers.Add(entry);

        customer.OnOrderCompleted += HandleOrderCompleted;
        customer.OnCustomerLeft += HandleCustomerLeft;
    }

    private void Update()
    {
        for (int i = _trackedCustomers.Count - 1; i >= 0; i--)
        {
            TrackedCustomer entry = _trackedCustomers[i];

            if (entry.customer.State != CustomerTesting.CustomerState.WaitingForOrder)
                continue;

            entry.remainingTime -= Time.deltaTime;

            float ratio = entry.remainingTime / entry.initialTime;
            OnCustomerMoodChanged?.Invoke(entry.customer, GetMood(ratio));

            if (entry.remainingTime <= 0f)
            {
                entry.customer.Leave(false);
                RemoveTrackedCustomer(entry.customer);
            }
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
        customer.OnOrderCompleted -= HandleOrderCompleted;
        customer.OnCustomerLeft -= HandleCustomerLeft;

        for (int i = 0; i < _trackedCustomers.Count; i++)
        {
            if (_trackedCustomers[i].customer == customer)
            {
                _trackedCustomers.RemoveAt(i);
                break;
            }
        }
    }
}