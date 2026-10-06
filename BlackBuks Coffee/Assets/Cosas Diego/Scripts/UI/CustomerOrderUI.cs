using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerOrderUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI orderText; // O usa TMPro.TextMeshProUGUI si usas TextMeshPro

    private void Update()
    {
        CustomerTesting[] customers = FindObjectsByType<CustomerTesting>(FindObjectsSortMode.None);

        string logText = "<b>PEDIDOS EN CURSO:</b>\n";
        bool activeCustomerFound = false;

        foreach (CustomerTesting customer in customers)
        {
            if (customer.State == CustomerTesting.CustomerState.WaitingForOrder)
            {
                activeCustomerFound = true;
                logText += $"• Pide: {string.Join(" + ", customer.PendingOrder)}\n";
            }
        }

        if (!activeCustomerFound)
        {
            logText += "Esperando clientes...";
        }

        if (orderText != null)
        {
            orderText.text = logText;
        }
    }
}