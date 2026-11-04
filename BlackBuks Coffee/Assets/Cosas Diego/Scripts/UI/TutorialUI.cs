using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _skipButton;

// En TutorialUI.cs
    private void OnEnable()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnTutorialStepChanged += UpdateTutorialUI;
            
            // CORRECCIÓN: Actualiza el texto inmediatamente con el paso actual
            UpdateTutorialUI(TutorialManager.Instance.CurrentStep);
        }
    
        if (_nextButton != null)
        {
            _nextButton.onClick.RemoveAllListeners();
            _nextButton.onClick.AddListener(OnNextButtonClicked);
        }
    
        if (_skipButton != null)
        {
            _skipButton.onClick.RemoveAllListeners();
            _skipButton.onClick.AddListener(OnSkipButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnTutorialStepChanged -= UpdateTutorialUI;
        }
    }

    private void UpdateTutorialUI(TutorialStep currentStep)
    {
        if (_nextButton != null)
        {
            _nextButton.gameObject.SetActive(currentStep == TutorialStep.Welcome);
        }

        switch (currentStep)
        {
            case TutorialStep.Welcome:
                SetInstruction("btn_tutorial_welcome", "¡Bienvenido a BlackBkus Coffee! Presiona Continuar para aprender las mecánicas básicas.");
                break;

            case TutorialStep.MoveToRegister:
                SetInstruction("btn_tutorial_move", "Usa WASD o las Flechas para moverte hacia la caja registradora.");
                break;

            case TutorialStep.TakeOrder:
                SetInstruction("btn_tutorial_order", "Acércate al cliente en la caja y presiona E para tomar su orden.");
                break;

            case TutorialStep.PrepareCoffee:
                SetInstruction("btn_tutorial_prepare", "Ve a la máquina de café y presiona E para preparar la bebida solicitada.");
                break;

            case TutorialStep.ServeCustomer:
                SetInstruction("btn_tutorial_serve", "Regresa con el cliente y presiona E para entregarle su pedido.");
                break;

            case TutorialStep.Completed:
                SetInstruction("btn_tutorial_complete", "¡Excelente trabajo! Has aprendido lo básico.");
                break;
        }
    }

    private void SetInstruction(string localizationKey, string fallbackText)
    {
        if (_instructionText == null) return;

        if (LocalizationManager.Instance != null)
        {
            _instructionText.text = LocalizationManager.Instance.GetText(localizationKey);
        }
        else
        {
            _instructionText.text = fallbackText;
        }
    }

    private void OnNextButtonClicked()
    {
        if (TutorialManager.Instance != null && TutorialManager.Instance.CurrentStep == TutorialStep.Welcome)
        {
            TutorialManager.Instance.CompleteStep(TutorialStep.Welcome);
        }
    }

    private void OnSkipButtonClicked()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.CancelTutorial();
        }
    }
}