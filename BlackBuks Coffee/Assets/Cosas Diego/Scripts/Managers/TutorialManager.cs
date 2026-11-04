using System;
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager _instance;
    public static TutorialManager Instance => _instance;

    [SerializeField] private CustomerTesting _tutorialCustomerPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _registerPoint;
    [SerializeField] private Transform _exitPoint;

    [Header("Configuración de Cierre")]
    [SerializeField] private bool _autoRedirectOnComplete = false;
    [SerializeField] private float _autoRedirectDelay = 3.5f;

    private TutorialStep _currentStep = TutorialStep.Welcome;
    private CustomerTesting _activeTutorialCustomer;
    private bool _isTutorialActive;

    public TutorialStep CurrentStep => _currentStep;
    public bool IsTutorialActive => _isTutorialActive;

    public event Action<TutorialStep> OnTutorialStepChanged;
    public event Action OnTutorialCompleted;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTutorialSequence()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameState.Tutorial);
        }

        _isTutorialActive = true;
        SetStep(TutorialStep.Welcome);
    }

    public void SetStep(TutorialStep newStep)
    {
        _currentStep = newStep;
        OnTutorialStepChanged?.Invoke(_currentStep);

        switch (_currentStep)
        {
            case TutorialStep.Welcome:
                break;

            case TutorialStep.MoveToRegister:
                SpawnTutorialCustomer();
                break;

            case TutorialStep.TakeOrder:
                break;

            case TutorialStep.PrepareCoffee:
                break;

            case TutorialStep.ServeCustomer:
                break;

            case TutorialStep.Completed:
                if (_autoRedirectOnComplete)
                {
                    StartCoroutine(AutoRedirectRoutine());
                }
                break;
        }
    }

    public void CompleteStep(TutorialStep completedStep)
    {
        if (!_isTutorialActive || _currentStep != completedStep)
        {
            return;
        }

        int nextStepIndex = (int)_currentStep + 1;
        SetStep((TutorialStep)nextStepIndex);
    }

    public void OnNextButtonClicked()
    {
        if (!_isTutorialActive) return;

        if (_currentStep == TutorialStep.Welcome)
        {
            CompleteStep(TutorialStep.Welcome);
        }
        else if (_currentStep == TutorialStep.Completed)
        {
            FinishTutorialAndGoToLevels();
        }
    }

    private void SpawnTutorialCustomer()
    {
        if (_tutorialCustomerPrefab == null || _spawnPoint == null)
        {
            return;
        }

        _activeTutorialCustomer = Instantiate(_tutorialCustomerPrefab, _spawnPoint.position, _spawnPoint.rotation);
        _activeTutorialCustomer.Init(_registerPoint, _exitPoint, 999f, 1);

        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.AddToQueue(_activeTutorialCustomer);
        }
    }

    private IEnumerator AutoRedirectRoutine()
    {
        yield return new WaitForSeconds(_autoRedirectDelay);
        if (_isTutorialActive && _currentStep == TutorialStep.Completed)
        {
            FinishTutorialAndGoToLevels();
        }
    }

    public void FinishTutorialAndGoToLevels()
    {
        CleanUpTutorialCustomer();
        _isTutorialActive = false;
        OnTutorialCompleted?.Invoke();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OpenLevelSelection();
        }
    }

    public void CancelTutorial()
    {
        CleanUpTutorialCustomer();
        _isTutorialActive = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CloseTutorial();
        }
    }

    private void CleanUpTutorialCustomer()
    {
        if (_activeTutorialCustomer != null)
        {
            Destroy(_activeTutorialCustomer.gameObject);
            _activeTutorialCustomer = null;
        }
    }
}