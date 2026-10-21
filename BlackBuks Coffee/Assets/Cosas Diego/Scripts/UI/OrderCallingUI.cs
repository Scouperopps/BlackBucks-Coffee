using System;
using TMPro;
using UnityEngine;

public class OrderCallingUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Transform _pickupSpotTransform;

    public void OnSubmitCallCustomer()
    {
        if (_nameInputField == null || string.IsNullOrEmpty(_nameInputField.text))
        {
            return;
        }

        string inputCustomerName = _nameInputField.text.Trim();
        CustomerTesting targetCustomer = SeatManager.Instance.FindSeatedCustomerByName(inputCustomerName);

        if (targetCustomer != null)
        {
            targetCustomer.CallToOrderTable(_pickupSpotTransform.position);
            _nameInputField.text = "";
        }
    }
}