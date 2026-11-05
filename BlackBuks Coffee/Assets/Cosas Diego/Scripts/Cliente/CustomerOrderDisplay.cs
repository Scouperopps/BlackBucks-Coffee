
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerOrderDisplay : MonoBehaviour
{
    [Header("Pedido")]
    [SerializeField] private Transform _orderContainer;
    [SerializeField] private GameObject _colorIconPrefab;
    [Header("Sprites de los Pedidos")]
    [SerializeField] private Sprite _coffeeSprite;    // Representa el Rojo
    [SerializeField] private Sprite _milkshakeSprite; // Representa el Azul
    [SerializeField] private Sprite _cupcakeSprite;   // Representa el Verde

    [Header("Nombre del cliente")]
    [SerializeField] private TextMeshProUGUI _customerNameText;
    [Header("Estrellas de Puntuaci�n")]
    [SerializeField] private GameObject[] _starIcons;

    public void ShowOrder(IReadOnlyList<SphereColor> order)
{
    // 1. Limpiar iconos anteriores
    foreach (Transform child in _orderContainer)
    {
        Destroy(child.gameObject);
    }

    // 2. Crear los nuevos iconos con los sprites correspondientes
    foreach (SphereColor color in order)
    {
        GameObject iconObj = Instantiate(_colorIconPrefab, _orderContainer);
        
        // IMPORTANTE: Asegúrate de que el componente sea UnityEngine.UI.Image
        UnityEngine.UI.Image iconImage = iconObj.GetComponent<UnityEngine.UI.Image>();

        // Restablecer el color a blanco para que el sprite no se tiña de gris o negro
        iconImage.color = Color.white; 

        switch (color)
        {
            case SphereColor.Red:
                iconImage.sprite = _coffeeSprite;
                break;
            case SphereColor.Blue:
                iconImage.sprite = _milkshakeSprite;
                break;
            case SphereColor.Green:
                iconImage.sprite = _cupcakeSprite;
                break;
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

    private void ClearOrder()
    {
        if (_orderContainer == null)
            return;

        for (int i = _orderContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_orderContainer.GetChild(i).gameObject);
        }
    }
    public void UpdateStars(float score)
    {
        int activeStars = 1;

        if (score >= 81f) activeStars = 5;
        else if (score >= 61f) activeStars = 4;
        else if (score >= 41f) activeStars = 3;
        else if (score >= 21f) activeStars = 2;
        else activeStars = 1; // De 0 a 20 siempre tendr� 1 estrella visible

        if (_starIcons != null)
        {
            for (int i = 0; i < _starIcons.Length; i++)
            {
                if (_starIcons[i] != null)
                {
                    // Enciende la estrella si su �ndice es menor a las estrellas activas (0 a 4)
                    _starIcons[i].SetActive(i < activeStars);
                }
            }
        }
    }
}
