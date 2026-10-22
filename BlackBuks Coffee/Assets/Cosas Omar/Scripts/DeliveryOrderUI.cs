using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryOrderUI : MonoBehaviour
{
    [Header("Panel de entrega")]
    [SerializeField] private GameObject deliveryPanel;

    [Header("Elementos de la interfaz")]
    [SerializeField] private TMP_InputField customerNameInput;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button confirmButton;

    private OrderTable _currentTable;
    private OrderTable.StoredOrder _currentOrder;

    private void Awake()
    {
        if (deliveryPanel != null)
            deliveryPanel.SetActive(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmOrder);

        if (customerNameInput != null)
        {
            customerNameInput.onValueChanged.AddListener(UpdateNamePreview);
            customerNameInput.onSubmit.AddListener(HandleSubmit);
        }
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(ConfirmOrder);

        if (customerNameInput != null)
        {
            customerNameInput.onValueChanged.RemoveListener(UpdateNamePreview);
            customerNameInput.onSubmit.RemoveListener(HandleSubmit);
        }
    }

    public void OpenForOrder(
        OrderTable table,
        OrderTable.StoredOrder order)
    {
        if (deliveryPanel == null || table == null || order == null)
            return;

        _currentTable = table;
        _currentOrder = order;

        deliveryPanel.SetActive(true);

        if (customerNameInput != null)
        {
            customerNameInput.text = "";
            customerNameInput.ActivateInputField();
            customerNameInput.Select();
        }

        if (statusText != null)
            statusText.text = "Escribe el nombre del cliente.";
    }

    public void ClosePanel()
    {
        if (deliveryPanel != null)
            deliveryPanel.SetActive(false);

        _currentTable = null;
        _currentOrder = null;
    }

    private void UpdateNamePreview(string enteredName)
    {
        if (statusText == null)
            return;

        statusText.text = string.IsNullOrWhiteSpace(enteredName)
            ? "Escribe el nombre del cliente."
            : "Cliente: " + enteredName;
    }

    private void HandleSubmit(string enteredName)
    {
        ConfirmOrder();
    }

    private void Update()
    {
        if (deliveryPanel == null || !deliveryPanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ConfirmOrder();
        }
    }

    private void ConfirmOrder()
    {
        if (deliveryPanel == null || !deliveryPanel.activeSelf)
            return;

        if (_currentTable == null || _currentOrder == null)
        {
            if (statusText != null)
                statusText.text = "No hay ningún pedido seleccionado.";

            return;
        }

        string customerName = customerNameInput != null
            ? customerNameInput.text.Trim()
            : "";

        if (string.IsNullOrWhiteSpace(customerName))
        {
            if (statusText != null)
                statusText.text = "Debes escribir un nombre.";

            return;
        }

        bool assigned = _currentTable.TryAssignName(
            _currentOrder,
            customerName
        );

        if (assigned)
        {
            ClosePanel();
        }
        else
        {
            if (statusText != null)
            {
                statusText.text = _currentTable.LastAssignmentMessage;
            }

            if (customerNameInput != null)
            {
                customerNameInput.ActivateInputField();
                customerNameInput.Select();
            }
        }
    }
}