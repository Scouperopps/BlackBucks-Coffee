
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

    private readonly List<StoredOrder> _storedOrders =
        new List<StoredOrder>();

    public IReadOnlyList<StoredOrder> StoredOrders => _storedOrders;

    public string GetInteractionText()
    {
        return "Depositar pedido";
    }

    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory == null)
            return;

        if (!playerInventory.HasSphere())
        {
            Debug.Log("No llevas esferas para depositar.");
            return;
        }

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
            $"Pedido depositado en DeliveryTable. " +
            $"Esferas: {string.Join(" + ", newOrder.colors)}. " +
            $"Pedidos almacenados: {_storedOrders.Count}"
        );
    }
}
