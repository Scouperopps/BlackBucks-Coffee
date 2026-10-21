using UnityEngine;

[System.Serializable]
public class Seat
{
    public Transform seatTransform;

    private bool _isOccupied;
    public bool IsOccupied => _isOccupied;

    private CustomerTesting _currentCustomer;
    public CustomerTesting CurrentCustomer => _currentCustomer;

    public void OccupySeat(CustomerTesting customer)
    {
        _isOccupied = true;
        _currentCustomer = customer;
    }

    public void ReleaseSeat()
    {
        _isOccupied = false;
        _currentCustomer = null;
    }
}