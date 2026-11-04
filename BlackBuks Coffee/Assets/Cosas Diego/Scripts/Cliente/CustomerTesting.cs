using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerTesting : MonoBehaviour
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

    [Header("Movimiento")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private CustomerOrderDisplay _orderDisplay;
    
    [Header("Nombre del cliente")]
    [SerializeField]
    private string[] _customerNames =
    {
        "Daniel", "Omar", "Valentina", "Carlos", "Aime",
        "Mateo", "Michelle", "Diego", "Elena", "Andres"
    };

    private string _customerName;
    public string CustomerName => _customerName;

    private Vector3 _targetPosition;
    private readonly List<SphereColor> _pendingOrder = new List<SphereColor>();

    private CustomerState _state;
    private bool _wasSatisfied;
    private float _patience;

    [Header("Sistema de Puntos")]
    private float _currentScore = 80f;
    private float _currentDecayRate = 0f; 

    public float CurrentScore => _currentScore;

    public void ModifyScore(float amount)
    {
        _currentScore = Mathf.Clamp(_currentScore + amount, 0f, 100f);
    }

    public event Action<CustomerTesting, bool> OnCustomerLeft;
    public event Action<CustomerTesting> OnOrderCompleted;
    public event Action<CustomerTesting> OnArrivedAtOrderTable;

    public CustomerState State => _state;
    public IReadOnlyList<SphereColor> PendingOrder => _pendingOrder;

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

        _customerName = _customerNames.Length > 0 ? _customerNames[UnityEngine.Random.Range(0, _customerNames.Length)] : "Cliente";

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
            return;

        _targetPosition = waitingPosition;
        _state = CustomerState.MovingToWaitingPoint;
    }

    public void CallToOrderTable(Transform orderTablePoint)
    {
        if (_state != CustomerState.WaitingForOrder)
            return;

        if (orderTablePoint == null)
            return;

        _targetPosition = orderTablePoint.position;
        _state = CustomerState.MovingToOrderTable;

        // AL ESCRIBIR EL NOMBRE Y LLAMAR AL CLIENTE, SE COMPLETA EL PASO 'ServeCustomer'
        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialActive)
        {
            if (TutorialManager.Instance.CurrentStep == TutorialStep.ServeCustomer)
            {
                TutorialManager.Instance.CompleteStep(TutorialStep.ServeCustomer);
            }
        }
    }

    private void Update()
    {
        if (!CanUpdateState())
            return;

        if (_state == CustomerState.WaitingAtRegister || _state == CustomerState.WaitingForOrder)
        {
            _currentScore -= _currentDecayRate * Time.deltaTime;
            _currentScore = Mathf.Max(0f, _currentScore); // Evitar puntaje negativo
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
                MoveTowardsTarget(_targetPosition, OnArrivedAtOrderTablePosition);
                break;

            case CustomerState.Leaving:
                if (_exitPoint != null)
                    MoveTowardsTarget(_exitPoint.position, OnArrivedAtExit);
                break;
        }
    }

    private bool CanUpdateState()
    {
        if (GameManager.Instance == null) return false;
        return GameManager.Instance.IsPlaying || GameManager.Instance.CurrentState == GameState.Tutorial;
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

    private void OnArrivedAtRegister()
    {
        _state = CustomerState.WaitingAtRegister;

        // Calcular pérdida proporcional: (Puntos actuales / Tiempo de paciencia)
        _currentDecayRate = _currentScore / _patience;

        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.StartTrackingCustomer(this, _patience);
        }
    }

    private void OnArrivedAtWaitingPoint()
    {
        _state = CustomerState.WaitingForOrder;
        // Eliminamos la llamada antigua al Manager, ya lo estamos trackeando
    }

    private void OnArrivedAtOrderTablePosition()
    {
        OnArrivedAtOrderTable?.Invoke(this);
        Leave(true);
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

        // Si se acaba el tiempo, enviar los puntos restantes a la suma global
        if (!satisfied && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(_currentScore);
            Debug.Log($"El cliente se fue sin orden. Se sumaron sus {_currentScore} puntos.");
        }
    }

    /*public void Interact(PlayerInventory playerInventory)
    {
        if (_state == CustomerState.WaitingAtRegister)
        {
            if (CustomerManager.Instance == null) return;
            if (!CustomerManager.Instance.CanServeAtRegister(this)) return;

            if (_orderDisplay != null)
            {
                _orderDisplay.ShowCustomerName(_customerName);
            }

            CustomerManager.Instance.ServeAtRegister(this);

            if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialActive)
            {
                if (TutorialManager.Instance.CurrentStep == TutorialStep.TakeOrder)
                {
                    TutorialManager.Instance.CompleteStep(TutorialStep.TakeOrder);
                }
            }
            return;
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
                return _wasSatisfied ? "Satisfied customer" : "Angry customer";

            default:
                return "";
        }
    }*/
    public void TakeOrderFromRegister()
    {
        if (_state != CustomerState.WaitingAtRegister)
            return;

        ModifyScore(30f);

        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.RefreshCustomerPatience(this, _patience);

            // Recalcular la tasa de pérdida con los nuevos puntos y el nuevo tiempo
            float extraTime = Mathf.Max(0, _pendingOrder.Count - 1) * 5f;
            float newPatience = _patience + extraTime;
            _currentDecayRate = _currentScore / newPatience;
        }

        if (_orderDisplay != null)
        {
            _orderDisplay.ShowCustomerName(_customerName);
            _orderDisplay.ShowOrder(_pendingOrder);
        }

        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.ServeAtRegister(this);
        }

        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialActive)
        {
            if (TutorialManager.Instance.CurrentStep == TutorialStep.TakeOrder)
            {
                TutorialManager.Instance.CompleteStep(TutorialStep.TakeOrder);
            }
        }
    }
}