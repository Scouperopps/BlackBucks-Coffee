using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerTesting : MonoBehaviour, IInteractable
{
    public enum CustomerState
    {
        MovingToPoint,
        WaitingForOrder,
        Leaving
    }

    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private Transform _customerPoint;
    [SerializeField] private Transform _exitPoint;

    private readonly List<SphereColor> _pendingOrder = new List<SphereColor>();
    private CustomerState _state;
    private bool _wasSatisfied;
    private float _patience;

    public event Action<CustomerTesting, bool> OnCustomerLeft;
    public event Action<CustomerTesting> OnOrderCompleted;

    public CustomerState State => _state;
    public IReadOnlyList<SphereColor> PendingOrder => _pendingOrder;

    public void Init(Transform customerPoint, Transform exitPoint, float patience, int maxItems)
    {
        _customerPoint = customerPoint;
        _exitPoint = exitPoint;
        _patience = patience;
        GenerateOrder(maxItems);
    }

    private void Start()
    {
        _state = CustomerState.MovingToPoint;
    }

    private void Update()
    {
        switch (_state)
        {
            case CustomerState.MovingToPoint:
                MoveTowardsTarget(_customerPoint, OnArrivedAtCustomerPoint);
                break;

            case CustomerState.Leaving:
                MoveTowardsTarget(_exitPoint, OnArrivedAtExit);
                break;
        }
    }

    private void MoveTowardsTarget(Transform target, Action onArrived)
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            _moveSpeed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            onArrived?.Invoke();
        }
    }

    private void OnArrivedAtCustomerPoint()
    {
        _state = CustomerState.WaitingForOrder;
        CustomerManager.Instance.RegisterWaitingCustomer(this, _patience);
    }

    private void OnArrivedAtExit()
    {
        OnCustomerLeft?.Invoke(this, _wasSatisfied);
        Destroy(gameObject);
    }

    private void GenerateOrder(int maxItems)
    {
        _pendingOrder.Clear();

        int itemCount = UnityEngine.Random.Range(1, Mathf.Max(1, maxItems) + 1);
        int colorCount = Enum.GetValues(typeof(SphereColor)).Length;

        for (int i = 0; i < itemCount; i++)
        {
            _pendingOrder.Add((SphereColor)UnityEngine.Random.Range(0, colorCount));
        }
    }

    public void Leave(bool satisfied)
    {
        _wasSatisfied = satisfied;
        _state = CustomerState.Leaving;
    }

    public void Interact(PlayerInventory playerInventory)
    {
        if (_state != CustomerState.WaitingForOrder)
            return;

        if (playerInventory == null || !playerInventory.HasSphere())
            return;

        SphereColor playerSphere = playerInventory.GetSphereColor();

        if (!_pendingOrder.Contains(playerSphere))
            return;

        playerInventory.RemoveSphere();
        _pendingOrder.Remove(playerSphere);

        if (_pendingOrder.Count == 0)
        {
            OnOrderCompleted?.Invoke(this);
            Leave(true);
        }
    }

    public string GetInteractionText()
    {
        switch (_state)
        {
            case CustomerState.MovingToPoint:
                return "Customer arriving";
            case CustomerState.Leaving:
                return _wasSatisfied ? "Customer satisfied" : "Customer angry";
            default:
                return "Deliver sphere (missing: " + string.Join(", ", _pendingOrder) + ")";
        }
    }
}