using UnityEngine;

public class CashRegister : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInventory playerInventory)
    {
        if (CustomerManager.Instance == null) return;

        CustomerTesting firstCustomer = CustomerManager.Instance.GetFirstCustomerInQueue();

        // Verificamos que haya un cliente y que ya esté esperando en la caja
        if (firstCustomer != null && firstCustomer.State == CustomerTesting.CustomerState.WaitingAtRegister)
        {
            firstCustomer.TakeOrderFromRegister();
            Debug.Log("Orden tomada a través de la computadora.");
        }
        else
        {
            Debug.Log("No hay clientes listos para ordenar.");
        }
    }

    public string GetInteractionText()
    {
        if (CustomerManager.Instance == null) return "Caja";

        CustomerTesting firstCustomer = CustomerManager.Instance.GetFirstCustomerInQueue();

        if (firstCustomer != null && firstCustomer.State == CustomerTesting.CustomerState.WaitingAtRegister)
        {
            return "Tomar orden";
        }

        return "Esperando clientes...";
    }
}