using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryOrderUI : MonoBehaviour
{
    public enum ComboButton
    {
        LT,
        RT,
        X,
        Y
    }

    [System.Serializable]
    public class CustomerComboGuideUI
    {
        public GameObject rowRoot;
        public TextMeshProUGUI customerNameText;
        public Image[] buttonIcons = new Image[4]; // Hasta 4 iconos de botón (LT, RT, X, Y)
    }

    [Header("Panel de entrega")]
    [SerializeField] private GameObject deliveryPanel;

    [Header("Elementos de la interfaz (Teclado)")]
    [SerializeField] private TMP_InputField customerNameInput;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button confirmButton;

    [Header("Elementos de la interfaz (Control Xbox - Combos GTA)")]
    [SerializeField] private GameObject gamepadComboContainer;
    [SerializeField] private TextMeshProUGUI currentComboSequenceText;
    [SerializeField] private CustomerComboGuideUI[] comboGuideRows;

    [Header("Sprites de Botones Xbox")]
    [SerializeField] private Sprite ltSprite;
    [SerializeField] private Sprite rtSprite;
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite ySprite;

    [Header("Opciones de Combo")]
    [Tooltip("Si está activo, al completar el combo del cliente se entrega el pedido automáticamente sin tener que presionar A.")]
    [SerializeField] private bool autoConfirmOnComboMatch = true;

    private OrderTable _currentTable;
    private OrderTable.StoredOrder _currentOrder;
    public static bool IsOpen { get; private set; }
    public static int LastFrameClosed { get; private set; }

    // Combos únicos por cliente (ninguno es prefijo de otro)
    private readonly Dictionary<string, ComboButton[]> _customerCombos = new Dictionary<string, ComboButton[]>
    {
        { "Michelle",  new[] { ComboButton.LT, ComboButton.RT, ComboButton.X, ComboButton.Y } },
        { "Omar",      new[] { ComboButton.RT, ComboButton.RT } },
        { "Aime",      new[] { ComboButton.LT, ComboButton.LT } },
        { "Mateo",     new[] { ComboButton.X,  ComboButton.X,  ComboButton.Y } },
        { "Diego",     new[] { ComboButton.Y,  ComboButton.Y } },
        { "Daniel",    new[] { ComboButton.LT, ComboButton.X,  ComboButton.X } },
        { "Valentina", new[] { ComboButton.RT, ComboButton.Y,  ComboButton.X } },
        { "Carlos",    new[] { ComboButton.X,  ComboButton.Y,  ComboButton.LT } },
        { "Elena",     new[] { ComboButton.Y,  ComboButton.X,  ComboButton.RT } },
        { "Andres",    new[] { ComboButton.LT, ComboButton.Y,  ComboButton.Y } }
    };

    private readonly List<ComboButton> _enteredCombo = new List<ComboButton>();
    private string _matchedCustomerName = "";

    // Control de flanco para gatillos analógicos LT / RT
    private bool _ltWasDown;
    private bool _rtWasDown;

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

        BuildComboGuideUI();
    }

    private void OnEnable()
    {
        if (InputDeviceDetector.Instance != null)
            InputDeviceDetector.Instance.OnDeviceChanged += HandleDeviceChanged;
    }

    private void OnDisable()
    {
        if (InputDeviceDetector.Instance != null)
            InputDeviceDetector.Instance.OnDeviceChanged -= HandleDeviceChanged;
    }

    private void OnDestroy()
    {
        IsOpen = false;

        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(ConfirmOrder);

        if (customerNameInput != null)
        {
            customerNameInput.onValueChanged.RemoveListener(UpdateNamePreview);
            customerNameInput.onSubmit.RemoveListener(HandleSubmit);
        }
    }

    private void BuildComboGuideUI()
    {
        if (comboGuideRows == null) return;

        int index = 0;
        foreach (var entry in _customerCombos)
        {
            if (index >= comboGuideRows.Length) break;

            CustomerComboGuideUI row = comboGuideRows[index];
            if (row != null && row.rowRoot != null)
            {
                row.rowRoot.SetActive(true);
                if (row.customerNameText != null)
                    row.customerNameText.text = entry.Key;

                ComboButton[] sequence = entry.Value;
                for (int i = 0; i < row.buttonIcons.Length; i++)
                {
                    if (row.buttonIcons[i] == null) continue;

                    if (i < sequence.Length)
                    {
                        row.buttonIcons[i].gameObject.SetActive(true);
                        row.buttonIcons[i].sprite = GetSpriteForButton(sequence[i]);
                    }
                    else
                    {
                        row.buttonIcons[i].gameObject.SetActive(false);
                    }
                }
            }
            index++;
        }
    }

    private Sprite GetSpriteForButton(ComboButton btn)
    {
        switch (btn)
        {
            case ComboButton.LT: return ltSprite;
            case ComboButton.RT: return rtSprite;
            case ComboButton.X: return xSprite;
            case ComboButton.Y: return ySprite;
            default: return null;
        }
    }

    public void OpenForOrder(OrderTable table, OrderTable.StoredOrder order)
    {
        if (deliveryPanel == null || table == null || order == null)
            return;

        _currentTable = table;
        _currentOrder = order;
        _enteredCombo.Clear();
        _matchedCustomerName = "";

        deliveryPanel.SetActive(true);
        IsOpen = true;

        bool usingGamepad = InputDeviceDetector.Instance != null && InputDeviceDetector.Instance.IsUsingGamepad;
        ApplyInputModeUI(usingGamepad);
    }

    private void HandleDeviceChanged(bool usingGamepad)
    {
        if (!IsOpen) return;
        ApplyInputModeUI(usingGamepad);
    }

    private void ApplyInputModeUI(bool usingGamepad)
    {
        if (customerNameInput != null)
            customerNameInput.gameObject.SetActive(!usingGamepad);

        if (gamepadComboContainer != null)
            gamepadComboContainer.SetActive(usingGamepad);

        if (usingGamepad)
        {
            UpdateComboFeedbackUI();
        }
        else
        {
            if (customerNameInput != null)
            {
                customerNameInput.text = "";
                customerNameInput.ActivateInputField();
                customerNameInput.Select();
            }

            if (statusText != null)
                statusText.text = "Escribe el nombre del cliente.";
        }
    }

    public void ClosePanel()
    {
        if (deliveryPanel != null)
            deliveryPanel.SetActive(false);

        LastFrameClosed = Time.frameCount;
        IsOpen = false;

        _currentTable = null;
        _currentOrder = null;
        _enteredCombo.Clear();
        _matchedCustomerName = "";
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

        bool usingGamepad = InputDeviceDetector.Instance != null && InputDeviceDetector.Instance.IsUsingGamepad;

        if (usingGamepad)
        {
            HandleGamepadComboInput();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePanel();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ConfirmOrder();
            }
        }
    }

    private void HandleGamepadComboInput()
    {
        ComboButton? pressedButton = ReadComboButtonDown();

        if (pressedButton.HasValue)
        {
            _enteredCombo.Add(pressedButton.Value);
            EvaluateComboMatch();
            UpdateComboFeedbackUI();

            if (autoConfirmOnComboMatch && !string.IsNullOrEmpty(_matchedCustomerName))
            {
                ConfirmOrder();
            }
            return;
        }

        // Botón B: Borrar último botón del combo o cerrar el panel si está vacío
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if (_enteredCombo.Count > 0)
            {
                _enteredCombo.RemoveAt(_enteredCombo.Count - 1);
                EvaluateComboMatch();
                UpdateComboFeedbackUI();
            }
            else
            {
                ClosePanel();
            }
            return;
        }

        // Botón A: Confirmar manualmente (si autoConfirmOnComboMatch está desactivado)
        if (Input.GetKeyDown(KeyCode.JoystickButton0) && !string.IsNullOrEmpty(_matchedCustomerName))
        {
            ConfirmOrder();
        }
    }

    private ComboButton? ReadComboButtonDown()
    {
        // 1. Botones digitales X e Y
        if (Input.GetKeyDown(KeyCode.JoystickButton2)) return ComboButton.X;
        if (Input.GetKeyDown(KeyCode.JoystickButton3)) return ComboButton.Y;

        // 2. Gatillos LT y RT (con detección de flanco para que cuenten una sola pulsación)
        bool ltDown = IsLeftTriggerPressed() || Input.GetKey(KeyCode.JoystickButton4); // Respaldo con LB
        bool rtDown = IsRightTriggerPressed() || Input.GetKey(KeyCode.JoystickButton5); // Respaldo con RB

        bool ltJustPressed = ltDown && !_ltWasDown;
        bool rtJustPressed = rtDown && !_rtWasDown;

        _ltWasDown = ltDown;
        _rtWasDown = rtDown;

        if (ltJustPressed) return ComboButton.LT;
        if (rtJustPressed) return ComboButton.RT;

        return null;
    }

    private bool IsLeftTriggerPressed()
    {
        try
        {
            return Input.GetAxisRaw("LT") > 0.5f;
        }
        catch
        {
            return false;
        }
    }

    private bool IsRightTriggerPressed()
    {
        try
        {
            return Input.GetAxisRaw("RT") > 0.5f;
        }
        catch
        {
            return false;
        }
    }

    private void EvaluateComboMatch()
    {
        _matchedCustomerName = "";

        // Verificar si el combo ingresado coincide exactamente con algún cliente
        foreach (var entry in _customerCombos)
        {
            ComboButton[] target = entry.Value;
            if (target.Length != _enteredCombo.Count) continue;

            bool exactMatch = true;
            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] != _enteredCombo[i])
                {
                    exactMatch = false;
                    break;
                }
            }

            if (exactMatch)
            {
                _matchedCustomerName = entry.Key;
                return;
            }
        }

        // Si ya ingresó 4 botones y no coincidió con nadie, reiniciar automáticamente
        if (_enteredCombo.Count >= 4)
        {
            _enteredCombo.Clear();
            if (statusText != null)
                statusText.text = "Combo incorrecto. Intenta de nuevo.";
        }
    }

    private void UpdateComboFeedbackUI()
    {
        if (currentComboSequenceText != null)
        {
            currentComboSequenceText.text = _enteredCombo.Count == 0
                ? "Ingresa el combo del cliente..."
                : string.Join(" - ", _enteredCombo);
        }

        if (statusText != null)
        {
            if (!string.IsNullOrEmpty(_matchedCustomerName))
                statusText.text = $"¡Llamando a {_matchedCustomerName}!";
            else if (_enteredCombo.Count > 0)
                statusText.text = "Combo: " + string.Join(" + ", _enteredCombo) + "  (B: Borrar)";
            else
                statusText.text = "Ingresa el combo del cliente (B: Salir)";
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

        bool usingGamepad = InputDeviceDetector.Instance != null && InputDeviceDetector.Instance.IsUsingGamepad;

        string customerName = usingGamepad
            ? _matchedCustomerName
            : (customerNameInput != null ? customerNameInput.text.Trim() : "");

        if (string.IsNullOrWhiteSpace(customerName))
        {
            if (statusText != null)
                statusText.text = usingGamepad ? "Completa un combo válido." : "Debes escribir un nombre.";

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

            if (usingGamepad)
            {
                // Si el cliente del combo no está esperando en la tienda, limpiar el combo para intentar otro
                _enteredCombo.Clear();
                _matchedCustomerName = "";
                UpdateComboFeedbackUI();
            }
            else if (customerNameInput != null)
            {
                customerNameInput.ActivateInputField();
                customerNameInput.Select();
            }
        }
    }
}