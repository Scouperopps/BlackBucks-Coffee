using System.Collections.Generic;
using UnityEngine;

public class PickUpCounter : MonoBehaviour
{
    private static PickUpCounter _instance;
    public static PickUpCounter Instance => _instance;

    public Transform pickupSpot;
    public List<GameObject> readyCoffees = new List<GameObject>();

    [SerializeField] private GameObject _coffeePrefab;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoffeeToCounter(Color coffeeColor)
    {
        Vector3 spawnPosition = pickupSpot.position + (Vector3.up * (readyCoffees.Count * 0.2f));
        GameObject newCoffee = Instantiate(_coffeePrefab, spawnPosition, Quaternion.identity);
        newCoffee.GetComponent<Renderer>().material.color = coffeeColor;
        readyCoffees.Add(newCoffee);
    }

    public GameObject TakeCoffeeForCustomer(CustomerTesting customer)
    {
        if (readyCoffees.Count > 0)
        {
            GameObject coffee = readyCoffees[0];
            readyCoffees.RemoveAt(0);
            return coffee;
        }
        return null;
    }
}