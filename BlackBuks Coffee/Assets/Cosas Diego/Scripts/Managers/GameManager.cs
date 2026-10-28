using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private GameState _currentState;
    private GameState _originState = GameState.MainMenu;

    public GameState CurrentState => _currentState;
    public bool IsPlaying => _currentState == GameState.Playing;

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

    private void Start()
    {
        SetGameState(GameState.MainMenu);

        if (ReputationManager.Instance != null)
        {
            ReputationManager.Instance.OnReputationDepleted += HandleReputationDepleted;
        }
    }

    private void OnDestroy()
    {
        if (ReputationManager.Instance != null)
        {
            ReputationManager.Instance.OnReputationDepleted -= HandleReputationDepleted;
        }
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
            else if (_currentState == GameState.LevelSelection)
            {
                CloseLevelSelection();
            }
        }
    }

    public void SetGameState(GameState newState)
    {
        _currentState = newState;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.DisplayPanelByState(_currentState);
        }
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

    public void OpenLevelSelection()
    {
        if (_currentState == GameState.MainMenu || _currentState == GameState.Paused)
        {
            _originState = _currentState;
        }
        SetGameState(GameState.LevelSelection);
    }

    public void CloseLevelSelection()
    {
        SetGameState(_originState);
    }

    public void OpenSettings()
    {
        if (_currentState == GameState.MainMenu || _currentState == GameState.Paused || _currentState == GameState.LevelSelection)
        {
            _originState = _currentState;
        }
        SetGameState(GameState.Settings);
    }

    public void CloseSettings()
    {
        SetGameState(_originState);
    }

    public void OpenTutorial()
    {
        if (_currentState == GameState.MainMenu || _currentState == GameState.Paused)
        {
            _originState = _currentState;
        }
        SetGameState(GameState.Tutorial);
    }

    public void CloseTutorial()
    {
        SetGameState(_originState);
    }

    public void TriggerVictory()
    {
        SetGameState(GameState.Victory);
    }

    public void TriggerDefeat()
    {
        SetGameState(GameState.Defeat);
    }

    public void EndGame(bool isVictory)
    {
        if (isVictory)
        {
            TriggerVictory();
        }
        else
        {
            TriggerDefeat();
        }
    }

    private void HandleReputationDepleted()
    {
        TriggerDefeat();
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