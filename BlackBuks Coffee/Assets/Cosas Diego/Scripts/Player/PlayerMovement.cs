using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        // Si no se cumplen las condiciones para mover al jugador, salimos
        if (!CanMove()) return;

        Move();
    }

    private bool CanMove()
    {
        // NUEVO: Si la interfaz de entrega está abierta, el jugador no se mueve.
        if (DeliveryOrderUI.IsOpen) return false;

        if (GameManager.Instance == null) return false;

        // 1. Permite movimiento durante la partida normal
        if (GameManager.Instance.IsPlaying) return true;

        // 2. Permite movimiento durante el Tutorial (salvo en el mensaje inicial)
        if (GameManager.Instance.CurrentState == GameState.Tutorial)
        {
            if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialActive)
            {
                // Congela al personaje SOLO en el mensaje de Bienvenida
                return TutorialManager.Instance.CurrentStep != TutorialStep.Welcome;
            }
            return true;
        }

        return false;
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(horizontal, 0f, vertical);

        if (input.magnitude > 1f)
        {
            input.Normalize();
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movement = cameraForward * vertical + cameraRight * horizontal;
        controller.Move(movement * moveSpeed * Time.deltaTime);

        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}