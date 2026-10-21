using System.Collections.Generic;
using UnityEngine;

public class CoffeeStation : MonoBehaviour
{
    [SerializeField] private List<Color> _currentIngredients = new List<Color>();

    public void AddColorIngredient(Color newColor)
    {
        _currentIngredients.Add(newColor);
    }

    public void CompleteAndPlaceCoffee()
    {
        if (_currentIngredients.Count == 0)
        {
            return;
        }

        Color finalColor = Color.clear;
        foreach (Color ingredientColor in _currentIngredients)
        {
            finalColor += ingredientColor;
        }
        finalColor /= _currentIngredients.Count;

        PickUpCounter.Instance.AddCoffeeToCounter(finalColor);
        _currentIngredients.Clear();
    }
}