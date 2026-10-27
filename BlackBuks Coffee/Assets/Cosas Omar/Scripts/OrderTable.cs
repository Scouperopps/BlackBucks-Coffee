
using System.Collections.Generic;
using UnityEngine;

public class OrderTable : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class StoredOrder
    {
        public List<SphereColor> colors = new List<SphereColor>();
        public string customerName = "";
    }

    [Header("Interfaz de entrega")]
    [SerializeField] private DeliveryOrderUI deliveryOrderUI;

    private readonly List<StoredOrder> _storedOrders =
        new List<StoredOrder>();

    public IReadOnlyList<StoredOrder> StoredOrders => _storedOrders;

    public string LastAssignmentMessage { get; private set; } = "";

    public string GetInteractionText()
    {
        return "Depositar pedido";
    }

    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory == null)
            return;

        if (playerInventory.HasSphere())
        {
            List<SphereColor> orderColors =
                new List<SphereColor>(playerInventory.CarriedSpheres);

            StoredOrder newOrder = new StoredOrder();
            newOrder.colors.AddRange(orderColors);

            foreach (SphereColor color in orderColors)
            {
                if (!playerInventory.TryRemoveSphere(color))
                {
                    Debug.LogError(
                        "No se pudo transferir una esfera a DeliveryTable."
                    );
                    return;
                }
            }

            _storedOrders.Add(newOrder);

            Debug.Log(
                $"Pedido depositado. Esferas: " +
                $"{string.Join(" + ", newOrder.colors)}. " +
                $"Pedidos almacenados: {_storedOrders.Count}"
            );

            OpenNamingPanel(newOrder);
            return;
        }

        StoredOrder unnamedOrder = _storedOrders.Find(
            order => string.IsNullOrWhiteSpace(order.customerName)
        );

        if (unnamedOrder != null)
        {
            OpenNamingPanel(unnamedOrder);
            return;
        }

        Debug.Log(
            "No llevas esferas y no hay pedidos pendientes de nombrar."
        );
    }

    private void OpenNamingPanel(StoredOrder order)
    {
        if (deliveryOrderUI == null)
        {
            Debug.LogError(
                "DeliveryOrderUI no está asignado en DeliveryTable."
            );
            return;
        }

        deliveryOrderUI.OpenForOrder(this, order);
    }

    public bool TryAssignName(StoredOrder order, string customerName)
    {
        LastAssignmentMessage = "";

        if (order == null || !_storedOrders.Contains(order))
        {
            LastAssignmentMessage = "El pedido ya no está disponible.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(order.customerName))
        {
            LastAssignmentMessage = "Este pedido ya tiene un nombre.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            LastAssignmentMessage = "Escribe el nombre del cliente.";
            return false;
        }

        if (CustomerManager.Instance == null)
        {
            LastAssignmentMessage = "No se encontró el sistema de clientes.";
            return false;
        }

        CustomerTesting customer =
            CustomerManager.Instance.FindWaitingCustomer(customerName);

        if (customer == null)
        {
            LastAssignmentMessage =
                "No hay ningún cliente con ese nombre en espera.";

            return false;
        }

        // Asociar el pedido al cliente.
        order.customerName = customer.CustomerName;

        // Escuchar el evento de llegada para retirar el pedido
        // cuando el cliente alcance la mesa.
        System.Action<CustomerTesting> onArrived = null;

        onArrived = arrivedCustomer =>
        {
            arrivedCustomer.OnArrivedAtOrderTable -= onArrived;
            _storedOrders.Remove(order);

            Debug.Log(
                $"El cliente {arrivedCustomer.CustomerName} " +
                "recogió su pedido."
            );
        };

        customer.OnArrivedAtOrderTable += onArrived;

        // Retirar al cliente de la fila B y enviarlo a la mesa.
        CustomerManager.Instance.RemoveFromWaitingQueue(customer);
        customer.CallToOrderTable(transform);

        LastAssignmentMessage =
            $"Pedido asignado a {customer.CustomerName}.";

        Debug.Log(LastAssignmentMessage);
        return true;
    }
}
