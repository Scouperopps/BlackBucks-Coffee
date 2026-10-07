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
    [SerializeField] private Transform _exitPoint;

    private Vector3 _targetPosition;
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
        _targetPosition = customerPoint != null ? customerPoint.position : transform.position;
        _exitPoint = exitPoint;
        _patience = patience;
        GenerateOrder(maxItems);
    }

    // Permite que la fila le asigne un lugar específico
    public void SetTargetQueuePosition(Vector3 queuePosition)
    {
        _targetPosition = queuePosition;
        if (_state != CustomerState.Leaving)
        {
            _state = CustomerState.MovingToPoint; // Hace que camine hacia su nuevo lugar en la fila
        }
    }

    private void Start()
    {
        _state = CustomerState.MovingToPoint;
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
        return;
        
        switch (_state)
        {
            case CustomerState.MovingToPoint:
                MoveTowardsTarget(_targetPosition, OnArrivedAtQueuePoint);
                break;

            case CustomerState.Leaving:
                if (_exitPoint != null)
                    MoveTowardsTarget(_exitPoint.position, OnArrivedAtExit);
                break;
        }
    }

    private void MoveTowardsTarget(Vector3 targetPosition, Action onArrived)
    {
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

    private void OnArrivedAtQueuePoint()
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

        for (int i = _pendingOrder.Count - 1; i >= 0; i--)
        {
            SphereColor requiredColor = _pendingOrder[i];

            if (playerInventory.TryRemoveSphere(requiredColor))
            {
                _pendingOrder.RemoveAt(i);
                break; // Entrega 1 esfera por interacción
            }
        }

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
                return "Cliente llegando";
            case CustomerState.Leaving:
                return _wasSatisfied ? "Cliente satisfecho" : "Cliente molesto";
            default:
                return "Entregar esfera (Falta: " + string.Join(", ", _pendingOrder) + ")";
        }
    }
}