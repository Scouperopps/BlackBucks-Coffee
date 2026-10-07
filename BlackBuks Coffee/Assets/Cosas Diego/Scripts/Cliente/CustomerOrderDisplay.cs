using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerOrderDisplay : MonoBehaviour
{
    [SerializeField] private Transform _orderContainer;
    [SerializeField] private GameObject _colorIconPrefab;
    [SerializeField] private Color _redColor = Color.red;
    [SerializeField] private Color _blueColor = Color.blue;
    [SerializeField] private Color _greenColor = Color.green;

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