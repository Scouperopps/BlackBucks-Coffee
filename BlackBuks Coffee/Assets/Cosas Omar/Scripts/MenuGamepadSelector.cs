using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuGamepadSelector : MonoBehaviour
{
    [Header("Botón inicial al abrir este panel")]
    [SerializeField] private Selectable defaultSelectedButton;

    private void OnEnable()
    {
        SelectDefaultButton();
    }

    public void SelectDefaultButton()
    {
        if (defaultSelectedButton == null || EventSystem.current == null)
            return;

        // Limpiar selección previa y forzar el foco en el botón de este menú
        EventSystem.current.SetSelectedGameObject(null);
        defaultSelectedButton.Select();
    }
}