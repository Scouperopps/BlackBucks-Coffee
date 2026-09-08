using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionRange = 1.5f;

    private IInteractable currentInteractable;

    private void Update()
    {
        DetectInteractable();

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact(GetComponent<PlayerInventory>());
        }
    }

    private void DetectInteractable()
    {
        currentInteractable = null;

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            interactionRange
        );

        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();

            if (interactable == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                collider.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            interactionRange
        );
    }
}