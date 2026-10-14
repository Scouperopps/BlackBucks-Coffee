using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory == null)
        {
            Debug.LogError("No se encontró PlayerInventory.");
            return;
        }

        if (!playerInventory.HasSphere())
        {
            Debug.Log("No tienes esferas para tirar.");
            return;
        }

        playerInventory.RemoveAllSpheres();

        Debug.Log("Has tirado todas las esferas a la basura.");
    }

    public string GetInteractionText()
    {
        return "Tirar ingredientes";
    }
}