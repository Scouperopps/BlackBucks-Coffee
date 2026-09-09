using System;
using UnityEngine;

public class CustomerTesting : MonoBehaviour, IInteractable
{
    public enum CustomerState
    {
        MovingToPoint,
        WaitingForOrder,
        Leaving
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Customer Point")]
    [SerializeField] private Transform customerPoint;

    [Header("Exit Point")]
    [SerializeField] private Transform exitPoint; // hacia dónde camina al irse

    [Header("Paciencia")]
    [SerializeField] private float tiempoMaximoDeEspera = 15f; // segundos esperando ser atendido

    // Evento opcional para que un manager reaccione (puntaje, spawn del siguiente cliente, etc.)
    // bool satisfecho = true si se le entregó el pedido correcto, false si se fue por impaciencia
    public event Action<CustomerTesting, bool> OnCustomerLeft;

    private SphereColor requestedColor;
    private CustomerState state;
    private float waitTimer;
    private bool wasSatisfied;

    private void Start()
    {
        GenerateOrder();
        state = CustomerState.MovingToPoint;
    }

    private void Update()
    {
        switch (state)
        {
            case CustomerState.MovingToPoint:
                MoveTowardsTarget(customerPoint, OnArrivedAtCustomerPoint);
                break;

            case CustomerState.WaitingForOrder:
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    LeaveAngry();
                }
                break;

            case CustomerState.Leaving:
                MoveTowardsTarget(exitPoint, OnArrivedAtExit);
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
            moveSpeed * Time.deltaTime
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
        state = CustomerState.WaitingForOrder;
        waitTimer = tiempoMaximoDeEspera;

        Debug.Log("Cliente ha llegado. Pedido: esfera " + requestedColor);
    }

    private void OnArrivedAtExit()
    {
        OnCustomerLeft?.Invoke(this, wasSatisfied);
        Destroy(gameObject);
    }

    private void GenerateOrder()
    {
        int randomColor = UnityEngine.Random.Range(0, 3);
        requestedColor = (SphereColor)randomColor;

        Debug.Log("Nuevo cliente. Pedido: esfera " + requestedColor);
    }

    private void LeaveAngry()
    {
        Debug.Log("Cliente se cansó de esperar y se va molesto.");
        wasSatisfied = false;
        state = CustomerState.Leaving;
    }

    private void LeaveSatisfied()
    {
        Debug.Log("¡Pedido correcto! Cliente recibió esfera " + requestedColor);
        wasSatisfied = true;
        state = CustomerState.Leaving;
    }

    public SphereColor GetRequestedColor()
    {
        return requestedColor;
    }

    public void Interact(PlayerInventory playerInventory)
    {
        if (state != CustomerState.WaitingForOrder)
        {
            if (state == CustomerState.MovingToPoint)
                Debug.Log("El cliente todavía está llegando.");
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogError("No se encontró PlayerInventory.");
            return;
        }

        if (!playerInventory.HasSphere())
        {
            Debug.Log("No tienes ninguna esfera para entregar.");
            return;
        }

        SphereColor playerSphere = playerInventory.GetSphereColor();

        if (playerSphere != requestedColor)
        {
            Debug.Log(
                "Pedido incorrecto. El cliente quiere " +
                requestedColor +
                " pero tienes " +
                playerSphere
            );
            return;
        }

        playerInventory.RemoveSphere();
        LeaveSatisfied();
    }

    public string GetInteractionText()
    {
        switch (state)
        {
            case CustomerState.MovingToPoint:
                return "Cliente llegando";
            case CustomerState.Leaving:
                return wasSatisfied ? "Cliente satisfecho" : "Cliente molesto";
            default:
                return "Entregar esfera " + requestedColor;
        }
    }
}