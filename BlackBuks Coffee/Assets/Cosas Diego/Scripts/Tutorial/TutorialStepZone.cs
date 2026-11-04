using UnityEngine;

public class TutorialStepZone : MonoBehaviour
{
    [Header("Configuración del Tutorial")]
    [SerializeField] private TutorialStep stepToComplete = TutorialStep.MoveToRegister;
    [SerializeField] private bool requireInteraction = false;

    private bool _isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Confirma si Unity detecta cualquier contacto físico
        Debug.Log($"[TutorialZone] Entró al trigger: {other.name}");

        if (IsPlayer(other))
        {
            _isPlayerInside = true;
            Debug.Log("[TutorialZone] Jugador identificado con éxito.");

            if (!requireInteraction)
            {
                TryCompleteStep();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            _isPlayerInside = false;
        }
    }

    private void Update()
    {
        if (requireInteraction && _isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            TryCompleteStep();
        }
    }

    private void TryCompleteStep()
    {
        if (TutorialManager.Instance == null)
        {
            Debug.LogWarning("[TutorialZone] Error: No existe TutorialManager.Instance en la escena.");
            return;
        }

        if (!TutorialManager.Instance.IsTutorialActive)
        {
            Debug.LogWarning("[TutorialZone] El tutorial NO está marcado como activo.");
            return;
        }

        Debug.Log($"[TutorialZone] Paso Zona: {stepToComplete} | Paso Actual Manager: {TutorialManager.Instance.CurrentStep}");

        if (TutorialManager.Instance.CurrentStep == stepToComplete)
        {
            Debug.Log($"[TutorialZone] ¡Paso {stepToComplete} completado exitosamente!");
            TutorialManager.Instance.CompleteStep(stepToComplete);
        }
    }

    private bool IsPlayer(Collider other)
    {
        // Busca el script o Tag en el objeto o en sus componentes padres
        return other.CompareTag("Player") || other.GetComponentInParent<PlayerMovement>() != null;
    }
}