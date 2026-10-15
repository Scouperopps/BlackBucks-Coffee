
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerOrderDisplay : MonoBehaviour
{
    [Header("Pedido")]
    [SerializeField] private Transform _orderContainer;
    [SerializeField] private GameObject _colorIconPrefab;
    [SerializeField] private Color _redColor = Color.red;
    [SerializeField] private Color _blueColor = Color.blue;
    [SerializeField] private Color _greenColor = Color.green;

    [Header("Nombre del cliente")]
    [SerializeField] private TextMeshProUGUI _customerNameText;

    public void ShowOrder(IReadOnlyList<SphereColor> order)
    {
        ClearOrder();

        foreach (SphereColor color in order)
        {
            GameObject icon = Instantiate(_colorIconPrefab, _orderContainer);
            Image image = icon.GetComponent<Image>();

            if (image != null)
            {
                image.color = GetColor(color);
            }
        }
    }

    public void ShowCustomerName(string customerName)
    {
        if (_customerNameText == null)
            return;

        _customerNameText.text = customerName;
        _customerNameText.gameObject.SetActive(true);
    }

    private Color GetColor(SphereColor color)
    {
        switch (color)
        {
            case SphereColor.Red:
                return _redColor;

            case SphereColor.Blue:
                return _blueColor;

            case SphereColor.Green:
                return _greenColor;

            default:
                return Color.white;
        }
    }

    private void ClearOrder()
    {
        if (_orderContainer == null)
            return;

        for (int i = _orderContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_orderContainer.GetChild(i).gameObject);
        }
    }
}
