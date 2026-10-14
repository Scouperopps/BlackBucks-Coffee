using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerOrderDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orderContainer;

    [Header("Icons")]
    [SerializeField] private GameObject colorIconPrefab;

    [Header("Colors")]
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color blueColor = Color.blue;
    [SerializeField] private Color greenColor = Color.green;

    public void ShowOrder(List<SphereColor> order)
    {
        ClearOrder();

        foreach (SphereColor color in order)
        {
            GameObject icon = Instantiate(
                colorIconPrefab,
                orderContainer
            );

            Image image = icon.GetComponent<Image>();

            if (image == null)
            {
                Debug.LogError(
                    "El prefab del icono no tiene un componente Image."
                );

                continue;
            }

            image.color = GetColor(color);
        }
    }

    private Color GetColor(SphereColor color)
    {
        switch (color)
        {
            case SphereColor.Red:
                return redColor;

            case SphereColor.Blue:
                return blueColor;

            case SphereColor.Green:
                return greenColor;

            default:
                return Color.white;
        }
    }

    private void ClearOrder()
    {
        if (orderContainer == null)
            return;

        for (int i = orderContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(orderContainer.GetChild(i).gameObject);
        }
    }
}