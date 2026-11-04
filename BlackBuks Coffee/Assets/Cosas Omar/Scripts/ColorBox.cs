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

            // COMPLETA EL PASO DE PREPARACIÓN EN EL TUTORIAL
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