using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionRange = 1.5f;

    private IInteractable currentInteractable;

    private void Update()
    {
        if (!CanInteract()) return;

        DetectInteractable();

        bool interactPressed = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton0);

        if (currentInteractable != null && interactPressed)
        {
            currentInteractable.Interact(GetComponent<PlayerInventory>());
        }
    }

    private bool CanInteract()
    {
        if(DeliveryOrderUI.IsOpen) return false;

        if (GameManager.Instance == null) return false;
        return GameManager.Instance.IsPlaying || GameManager.Instance.CurrentState == GameState.Tutorial;
    }

    private void DetectInteractable()
    {
        currentInteractable = null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            float distance = Vector3.Distance(transform.position, collider.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}