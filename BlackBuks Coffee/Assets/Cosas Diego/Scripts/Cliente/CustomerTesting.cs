using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerTesting : MonoBehaviour, IInteractable
{
    public enum CustomerState
    {
        MovingToPoint,
        WaitingAtRegister,
        MovingToWaitingPoint,
        WaitingForOrder,
        MovingToOrderTable,
        Leaving
    }

    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private CustomerOrderDisplay _orderDisplay;
    [SerializeField]
    private string[] _customerNames =
    {
        "Daniel",
        "Omar",
        "Valentina",
        "Carlos",
        "Aime",
        "Mateo",
        "Michelle",
        "Diego",
        "Elena",
        "Andres"
    };

    private string _customerName;
    public string CustomerName => _customerName;

    private Vector3 _targetPosition;
    private readonly List<SphereColor> _pendingOrder = new List<SphereColor>();

    private CustomerState _state;
    private bool _wasSatisfied;
    private float _patience;
    private float _maxPatience;

    public event Action<CustomerTesting, bool> OnCustomerLeft;
    public event Action<CustomerTesting> OnOrderCompleted;

    public CustomerState State => _state;
    public IReadOnlyList<SphereColor> PendingOrder => _pendingOrder;

    private const float ROTATION_SPEED = 10f;
    private const float ARRIVAL_THRESHOLD = 0.05f;

    public void Init(
        Transform customerPoint,
        Transform exitPoint,
        float patience,
        int maxItems)
    {
        _targetPosition = customerPoint != null
            ? customerPoint.position
            : transform.position;

        _exitPoint = exitPoint;
        _patience = patience;
        _maxPatience = patience > 0f ? patience : 1f;

        _customerName = _customerNames.Length > 0
            ? _customerNames[UnityEngine.Random.Range(0, _customerNames.Length)]
            : "Customer";

        GenerateOrder(maxItems);
        _state = CustomerState.MovingToPoint;
    }

    public void SetTargetQueuePosition(Vector3 queuePosition)
    {
        if (_state == CustomerState.Leaving ||
            _state == CustomerState.MovingToWaitingPoint ||
            _state == CustomerState.WaitingForOrder ||
            _state == CustomerState.MovingToOrderTable)
        {
            return;
        }

        _targetPosition = queuePosition;
        _state = CustomerState.MovingToPoint;
    }

    public void SetTargetWaitingPosition(Vector3 waitingPosition)
    {
        if (_state == CustomerState.Leaving)
        {
            return;
        }

        _targetPosition = waitingPosition;
        _state = CustomerState.MovingToWaitingPoint;
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
        {
            return;
        }

        if (_state == CustomerState.WaitingAtRegister || _state == CustomerState.WaitingForOrder)
        {
            _patience -= Time.deltaTime;
            if (_patience <= 0f)
            {
                if (ReputationManager.Instance != null)
                {
                    ReputationManager.Instance.ProcessCustomerLeftAngry();
                }
                Leave(false);
                return;
            }
        }

        switch (_state)
        {
            case CustomerState.MovingToPoint:
                MoveTowardsTarget(_targetPosition, OnArrivedAtRegister);
                break;

            case CustomerState.MovingToWaitingPoint:
                MoveTowardsTarget(_targetPosition, OnArrivedAtWaitingPoint);
                break;

            case CustomerState.MovingToOrderTable:
                break;

            case CustomerState.Leaving:
                if (_exitPoint != null)
                {
                    MoveTowardsTarget(_exitPoint.position, OnArrivedAtExit);
                }
                break;
        }
    }

    private void MoveTowardsTarget(
        Vector3 targetPosition,
        Action onArrivedAction)
    {
        targetPosition.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            _moveSpeed * Time.deltaTime
        );

        Vector3 movementDirection = targetPosition - transform.position;

        if (movementDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                ROTATION_SPEED * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, targetPosition) < ARRIVAL_THRESHOLD)
        {
            onArrivedAction?.Invoke();
        }
    }

    private void OnArrivedAtRegister()
    {
        _state = CustomerState.WaitingAtRegister;
    }

    private void OnArrivedAtWaitingPoint()
    {
        _state = CustomerState.WaitingForOrder;

        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.RegisterWaitingCustomer(
                this,
                _patience
            );
        }
    }

    private void OnArrivedAtExit()
    {
        OnCustomerLeft?.Invoke(this, _wasSatisfied);
        Destroy(gameObject);
    }

    private void GenerateOrder(int maxItems)
    {
        _pendingOrder.Clear();

        int itemCount = UnityEngine.Random.Range(
            1,
            Mathf.Max(1, maxItems) + 1
        );

        int colorCount = Enum.GetValues(typeof(SphereColor)).Length;

        for (int i = 0; i < itemCount; i++)
        {
            _pendingOrder.Add(
                (SphereColor)UnityEngine.Random.Range(0, colorCount)
            );
        }

        if (_orderDisplay != null)
        {
            _orderDisplay.ShowOrder(_pendingOrder);
        }
    }

    public void Leave(bool isSatisfied)
    {
        _wasSatisfied = isSatisfied;
        _state = CustomerState.Leaving;
    }

    public void Interact(PlayerInventory playerInventory)
    {
        if (_state == CustomerState.WaitingAtRegister)
        {
            if (CustomerManager.Instance == null)
            {
                return;
            }

            if (!CustomerManager.Instance.CanServeAtRegister(this))
            {
                return;
            }

            if (_orderDisplay != null)
            {
                _orderDisplay.ShowCustomerName(_customerName);
            }

            CustomerManager.Instance.ServeAtRegister(this);
            return;
        }

        if (_state != CustomerState.WaitingForOrder)
        {
            return;
        }

        if (playerInventory == null || !playerInventory.HasSphere())
        {
            return;
        }

        bool isOrderUpdated = false;

        for (int i = _pendingOrder.Count - 1; i >= 0; i--)
        {
            SphereColor requiredColor = _pendingOrder[i];

            if (playerInventory.TryRemoveSphere(requiredColor))
            {
                _pendingOrder.RemoveAt(i);
                isOrderUpdated = true;
                break;
            }
        }

        if (isOrderUpdated && _orderDisplay != null)
        {
            _orderDisplay.ShowOrder(_pendingOrder);
        }

        if (_pendingOrder.Count == 0)
        {
            float remainingPatienceRatio = Mathf.Clamp01(_patience / _maxPatience);

            if (ReputationManager.Instance != null)
            {
                ReputationManager.Instance.ProcessOrderDelivery(remainingPatienceRatio);
            }

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

            case CustomerState.WaitingAtRegister:
                return CustomerManager.Instance != null &&
                       CustomerManager.Instance.CanServeAtRegister(this)
                    ? "Attend customer"
                    : "Wait for your turn";

            case CustomerState.MovingToWaitingPoint:
                return "Going to waiting area";

            case CustomerState.WaitingForOrder:
                return "Deliver ingredient";

            case CustomerState.MovingToOrderTable:
                return "Collecting order";

            case CustomerState.Leaving:
                return _wasSatisfied
                    ? "Satisfied customer"
                    : "Angry customer";

            default:
                return "";
        }
    }
}