using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState;
    private GameState _originState = GameState.MainMenu; // Guarda si venimos de MainMenu o Paused

    public GameState CurrentState => _currentState;
    public bool IsPlaying => _currentState == GameState.Playing;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetGameState(GameState.MainMenu);
    }

    private void Update()
    {
        HandlePauseInput();
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (_currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else if (_currentState == GameState.Settings)
            {
                CloseSettings();
            }
            else if (_currentState == GameState.Tutorial)
            {
                CloseTutorial();
            }
        }
    }

    public void SetGameState(GameState newState)
    {
        _currentState = newState;
        UIManager.Instance.DisplayPanelByState(_currentState);
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
    }

    public void PauseGame()
    {
        SetGameState(GameState.Paused);
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
    }

    public void OpenSettings()
    {
        // Solo guardamos el origen si venimos de las pantallas principales
        if (_currentState == GameState.MainMenu || _currentState == GameState.Paused)
        {
            _originState = _currentState;
        }
        SetGameState(GameState.Settings);
    }

    public void CloseSettings()
    {
        SetGameState(_originState); // Regresa exactamente a MainMenu o Paused
    }

    public void OpenTutorial()
    {
        // Solo guardamos el origen si venimos de las pantallas principales
        if (_currentState == GameState.MainMenu || _currentState == GameState.Paused)
        {
            _originState = _currentState;
        }
        SetGameState(GameState.Tutorial);
    }

    public void CloseTutorial()
    {
        SetGameState(_originState); // Regresa exactamente a MainMenu o Paused
    }

    public void TriggerVictory()
    {
        SetGameState(GameState.Victory);
    }

    public void TriggerDefeat()
    {
        SetGameState(GameState.Defeat);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}