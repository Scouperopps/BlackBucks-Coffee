using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour, IInteractable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Customer Point")]
    [SerializeField] private Transform customerPoint;

    [SerializeField] private Transform customerExitPoint;

    [Header("Order")]
    [SerializeField] private int ingredientsPerOrder = 2;
    [SerializeField] private CustomerOrderDisplay orderDisplay;

    private List<SphereColor> requestedColors = new List<SphereColor>();

    private bool hasArrived = false;
    private bool isLeaving = false;

    private void Start()
    {
        GenerateOrder();
    }

    private void Update()
    {
        if (!hasArrived)
        {
            MoveToCustomerPoint();
        }
        else if (isLeaving)
        {
            MoveToExit();
        }
    }

    private void MoveToCustomerPoint()
    {
        if (customerPoint == null)
            return;

        Vector3 targetPosition = customerPoint.position;

        targetPosition.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            hasArrived = true;

            Debug.Log(
                "Cliente ha llegado. Pedido: " + GetOrderText()
            );
        }
    }

    private void MoveToExit()
    {
        if (customerExitPoint == null)
            return;

        Vector3 targetPosition = customerExitPoint.position;

        targetPosition.y = transform.position.y;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            Destroy(gameObject);
        }
    }

    private void GenerateOrder()
    {
        requestedColors.Clear();

        for (int i = 0; i < ingredientsPerOrder; i++)
        {
            int randomColor = Random.Range(0, 3);

            SphereColor color = (SphereColor)randomColor;

            requestedColors.Add(color);
        }

        Debug.Log(
            "Nuevo cliente. Pedido: " + GetOrderText()
        );

        if (orderDisplay != null)
        {
            orderDisplay.ShowOrder(requestedColors);
        }
    }

    public List<SphereColor> GetRequestedColors()
    {
        return requestedColors;
    }

    public void Interact(PlayerInventory playerInventory)
    {
        if (!hasArrived)
        {
            Debug.Log("El cliente todavía está llegando.");
            return;
        }

        if (isLeaving)
        {
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogError("No se encontró PlayerInventory.");
            return;
        }

        if (!playerInventory.HasSphere())
        {
            Debug.Log("No tienes ingredientes para entregar.");
            return;
        }

        List<SphereColor> playerSpheres = playerInventory.GetSpheres();

        if (!IsOrderCorrect(playerSpheres))
        {
            Debug.Log(
                "Pedido incorrecto. El cliente pidió: " +
                GetOrderText()
            );

            return;
        }

        Debug.Log(
            "¡Pedido correcto! El cliente recibió: " +
            GetOrderText()
        );

        playerInventory.RemoveAllSpheres();

        isLeaving = true;
    }

    private bool IsOrderCorrect(List<SphereColor> playerSpheres)
    {
        if (playerSpheres.Count != requestedColors.Count)
            return false;

        List<SphereColor> playerCopy =
            new List<SphereColor>(playerSpheres);

        List<SphereColor> orderCopy =
            new List<SphereColor>(requestedColors);

        foreach (SphereColor color in orderCopy)
        {
            if (!playerCopy.Contains(color))
                return false;

            playerCopy.Remove(color);
        }

        return playerCopy.Count == 0;
    }

    private string GetOrderText()
    {
        string orderText = "";

        for (int i = 0; i < requestedColors.Count; i++)
        {
            orderText += requestedColors[i];

            if (i < requestedColors.Count - 1)
            {
                orderText += " + ";
            }
        }

        return orderText;
    }

    public string GetInteractionText()
    {
        if (!hasArrived)
            return "Cliente llegando";

        if (isLeaving)
            return "Cliente satisfecho";

        return "Entregar bebida";
    }
}