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

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Customer Point")]
    [SerializeField] private Transform customerPoint;

    [Header("Exit Point")]
    [SerializeField] private Transform exitPoint; // hacia dónde camina al irse

    [Header("Paciencia")]
    [SerializeField] private float tiempoMaximoDeEspera = 15f; // segundos esperando ser atendido
    [SerializeField] private float tiempoExtraPorItem = 5f;    // segundos extra por cada esfera adicional del pedido

    [Header("Pedido")]
    [SerializeField, Min(1)] private int maxItemsPorPedido = 1; // el pedido tendrá entre 1 y este número de esferas

    // Evento opcional para que un manager reaccione (puntaje, spawn del siguiente cliente, etc.)
    // bool satisfecho = true si se le entregó el pedido completo, false si se fue por impaciencia
    public event Action<CustomerTesting, bool> OnCustomerLeft;

    // Esferas que todavía faltan por entregarle
    private readonly List<SphereColor> pendingOrder = new List<SphereColor>();
    private CustomerState state;
    private float waitTimer;
    private bool wasSatisfied;

    // El WaveManager lo llama justo después de instanciar al cliente
    public void Init(Transform point, Transform exit, float patience, int maxItems)
    {
        customerPoint = point;
        exitPoint = exit;
        tiempoMaximoDeEspera = patience;
        maxItemsPorPedido = Mathf.Max(1, maxItems);
    }

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

        // Más esferas en el pedido = más tiempo para completarlo
        waitTimer = tiempoMaximoDeEspera + tiempoExtraPorItem * (pendingOrder.Count - 1);

        Debug.Log("Cliente ha llegado. Pedido: " + string.Join(", ", pendingOrder));
    }

    private void OnArrivedAtExit()
    {
        OnCustomerLeft?.Invoke(this, wasSatisfied);
        Destroy(gameObject);
    }

    private void GenerateOrder()
    {
        pendingOrder.Clear();

        int itemCount = UnityEngine.Random.Range(1, maxItemsPorPedido + 1);
        int colorCount = Enum.GetValues(typeof(SphereColor)).Length;

        for (int i = 0; i < itemCount; i++)
        {
            pendingOrder.Add((SphereColor)UnityEngine.Random.Range(0, colorCount));
        }

        Debug.Log("Nuevo cliente. Pedido: " + string.Join(", ", pendingOrder));
    }

    private void LeaveAngry()
    {
        Debug.Log("Cliente se cansó de esperar y se va molesto.");
        wasSatisfied = false;
        state = CustomerState.Leaving;
    }

    private void LeaveSatisfied()
    {
        Debug.Log("¡Pedido completo! Cliente satisfecho.");
        wasSatisfied = true;
        state = CustomerState.Leaving;
    }

    // Por si más adelante quieres mostrar el pedido en un ticket o sobre la cabeza del cliente
    public IReadOnlyList<SphereColor> GetPendingColors()
    {
        return pendingOrder;
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

        // Si el color no está en lo que falta del pedido, no pasa nada
        if (!pendingOrder.Contains(playerSphere))
        {
            Debug.Log(
                "Ese color no lo pidió. Le falta: " +
                string.Join(", ", pendingOrder) +
                " pero tienes " +
                playerSphere
            );
            return;
        }

        playerInventory.RemoveSphere();
        pendingOrder.Remove(playerSphere); // quita solo una esfera de ese color

        if (pendingOrder.Count == 0)
        {
            LeaveSatisfied();
        }
        else
        {
            Debug.Log("Esfera entregada. Aún falta: " + string.Join(", ", pendingOrder));
        }
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
                return "Entregar esfera (falta: " + string.Join(", ", pendingOrder) + ")";
        }
    }
}