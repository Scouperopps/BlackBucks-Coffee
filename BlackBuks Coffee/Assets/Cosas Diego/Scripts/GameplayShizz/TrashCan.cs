using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory == null || !playerInventory.HasSphere())
            return;

        playerInventory.RemoveAllSpheres();
    }

    public string GetInteractionText()
    {
        return "Discard ingredients";
    }
}