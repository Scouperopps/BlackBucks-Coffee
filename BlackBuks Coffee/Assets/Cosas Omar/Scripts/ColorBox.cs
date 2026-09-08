using UnityEngine;

public class ColorBox : MonoBehaviour, IInteractable
{
    [Header("Box Settings")]
    [SerializeField] private SphereColor sphereColor;

    private PlayerInventory playerInventory;

    private void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory == null)
        {
            Debug.LogError("No se encontró PlayerInventory.");
            return;
        }

        bool sphereAdded = playerInventory.TryAddSphere(sphereColor);

        if (sphereAdded)
        {
            Debug.Log("Esfera " + sphereColor + " recogida de la caja.");
        }
    }

    public string GetInteractionText()
    {
        if (playerInventory != null && playerInventory.HasSphere())
        {
            return "Ya tienes una esfera";
        }

        return "Recoger esfera " + sphereColor;
    }
}