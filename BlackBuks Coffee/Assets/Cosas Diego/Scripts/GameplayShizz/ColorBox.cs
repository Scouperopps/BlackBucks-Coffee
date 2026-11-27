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
        if (playerInventory == null) return;

        bool sphereAdded = playerInventory.TryAddSphere(sphereColor);

        if (sphereAdded)
        {
            if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialActive)
            {
                if (TutorialManager.Instance.CurrentStep == TutorialStep.PrepareCoffee)
                {
                    TutorialManager.Instance.CompleteStep(TutorialStep.PrepareCoffee);
                }
            }
        }
    }

    public string GetInteractionText()
    {
        if (playerInventory != null && playerInventory.IsFull())
        {
            return "Inventario lleno";
        }
        return "Recoger esfera " + sphereColor;
    }
}