public interface IInteractable
{
    void Interact(PlayerInventory playerInventory);

    string GetInteractionText();
}