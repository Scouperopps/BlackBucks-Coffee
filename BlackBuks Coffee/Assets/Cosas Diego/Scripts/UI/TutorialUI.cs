using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _instructionText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _skipButton;

    private void OnEnable()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnTutorialStepChanged += UpdateTutorialUI;
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
            bool showNext = currentStep == TutorialStep.Welcome || currentStep == TutorialStep.Completed;
            _nextButton.gameObject.SetActive(showNext);
        }

        switch (currentStep)
        {
            case TutorialStep.Welcome:
                SetInstruction("btn_tutorial_welcome", "¡Bienvenido a BlackBuks Coffee! Presiona Continuar para aprender cómo atender la cafetería.");
                break;

            case TutorialStep.MoveToRegister:
                SetInstruction("btn_tutorial_move", "Usa WASD o las Flechas para acercarte a la caja registradora en el mostrador central.");
                break;

            case TutorialStep.TakeOrder:
                SetInstruction("btn_tutorial_order", "Párate frente a la caja registradora y presiona E para interactuar con el cliente y tomar su orden.");
                break;

            case TutorialStep.PrepareCoffee:
                SetInstruction("btn_tutorial_prepare", "Toma los productos del pedido con E: las tazas en el mostrador central, o los vasos azules y los muffins en la barra izquierda.");
                break;

            case TutorialStep.ServeCustomer:
                SetInstruction("btn_tutorial_serve", "Acércate al área de entregas en el mostrador, presiona E para escribir el nombre del cliente y entrega su pedido.");
                break;

            case TutorialStep.Completed:
                SetInstruction("btn_tutorial_complete", "¡Excelente trabajo! Ya dominas lo básico de BlackBuks Coffee.");
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
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnNextButtonClicked();
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