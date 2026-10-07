using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState;
    private GameState _previousState;

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
        }
    }

    public void SetGameState(GameState newState)
    {
        _previousState = _currentState;
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
        SetGameState(GameState.Settings);
    }

    public void CloseSettings()
    {
        SetGameState(_previousState == GameState.Settings ? GameState.MainMenu : _previousState);
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