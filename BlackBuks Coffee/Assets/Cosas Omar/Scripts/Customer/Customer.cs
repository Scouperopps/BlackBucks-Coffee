using UnityEngine;

public class Customer : MonoBehaviour, IInteractable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Customer Point")]
    [SerializeField] private Transform customerPoint;

    private SphereColor requestedColor;
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
                "Cliente ha llegado. Pedido: esfera " + requestedColor
            );
        }
    }

    private void GenerateOrder()
    {
        int randomColor = Random.Range(0, 3);

        requestedColor = (SphereColor)randomColor;

        Debug.Log(
            "Nuevo cliente. Pedido: esfera " + requestedColor
        );
    }

    public SphereColor GetRequestedColor()
    {
        return requestedColor;
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
            Debug.Log("No tienes ninguna esfera para entregar.");
            return;
        }

        SphereColor playerSphere = playerInventory.GetSphereColor();

        if (playerSphere != requestedColor)
        {
            Debug.Log(
                "Pedido incorrecto. El cliente quiere " +
                requestedColor +
                " pero tienes " +
                playerSphere
            );

            return;
        }

        Debug.Log(
            "¡Pedido correcto! Cliente recibió esfera " +
            requestedColor
        );

        playerInventory.RemoveSphere();

        isLeaving = true;
    }

    public string GetInteractionText()
    {
        if (!hasArrived)
            return "Cliente llegando";

        if (isLeaving)
            return "Cliente satisfecho";

        return "Entregar esfera " + requestedColor;
    }
}