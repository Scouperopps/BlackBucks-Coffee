using System;
using System.Collections.Generic;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    private static SeatManager _instance;
    public static SeatManager Instance => _instance;

    public List<Seat> seatList = new List<Seat>();
    public Transform exitPoint;

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

    public Seat GetAvailableSeat()
    {
        foreach (Seat seat in seatList)
        {
            if (!seat.IsOccupied)
            {
                return seat;
            }
        }
        return null;
    }

    public CustomerTesting FindSeatedCustomerByName(string customerName)
    {
        foreach (Seat seat in seatList)
        {
            if (seat.IsOccupied && seat.CurrentCustomer != null)
            {
                if (seat.CurrentCustomer.CustomerName.Equals(customerName, StringComparison.OrdinalIgnoreCase))
                {
                    return seat.CurrentCustomer;
                }
            }
        }
        return null;
    }
}