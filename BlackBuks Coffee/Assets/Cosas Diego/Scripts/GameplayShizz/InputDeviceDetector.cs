using System;
using UnityEngine;

public class InputDeviceDetector : MonoBehaviour
{
    public static InputDeviceDetector Instance { get; private set; }

    public bool IsUsingGamepad { get; private set; }
    public event Action<bool> OnDeviceChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 1. Detectar si se presionó algún botón del control de Xbox (botones 0 a 19) o se movieron sticks/gatillos
        if (IsGamepadInputDetected())
        {
            SetDevice(true);
            return;
        }

        // 2. Detectar si se presionó alguna tecla física del teclado o clic de ratón
        if (IsKeyboardOrMouseDetected())
        {
            SetDevice(false);
        }
    }

    private void SetDevice(bool usingGamepad)
    {
        if (IsUsingGamepad == usingGamepad) return;

        IsUsingGamepad = usingGamepad;
        OnDeviceChanged?.Invoke(IsUsingGamepad);
    }

    private bool IsGamepadInputDetected()
    {
        // Revisar botones principales del mando (A, B, X, Y, LB, RB, Back, Start, Sticks)
        for (int i = 0; i <= 15; i++)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton0 + i))
                return true;
        }

        // Revisar movimiento de los sticks principales
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.2f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.2f)
        {
            // Asegurarnos de que no sean las teclas WASD o flechas
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) &&
                !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) &&
                !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) &&
                !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsKeyboardOrMouseDetected()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            return true;

        if (Input.anyKeyDown)
        {
            // Excluir los botones del joystick para no generar falsos positivos
            for (int i = 0; i <= 19; i++)
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton0 + i))
                    return false;
            }
            return true;
        }

        return false;
    }
}