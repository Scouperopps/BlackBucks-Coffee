using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _levelSelectionPanel;
    [SerializeField] private GameObject _gameUIPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatPanel;

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

    public void DisplayPanelByState(GameState state)
    {
        HideAllPanels();

        switch (state)
        {
            case GameState.MainMenu:
                if (_mainMenuPanel != null) _mainMenuPanel.SetActive(true);
                break;

            case GameState.LevelSelection:
                if (_levelSelectionPanel != null) _levelSelectionPanel.SetActive(true);
                break;

            case GameState.Playing:
                if (_gameUIPanel != null) _gameUIPanel.SetActive(true);
                break;

            case GameState.Paused:
                if (_gameUIPanel != null) _gameUIPanel.SetActive(true);
                if (_pausePanel != null) _pausePanel.SetActive(true);
                break;

            case GameState.Settings:
                if (_settingsPanel != null) _settingsPanel.SetActive(true);
                break;

            case GameState.Tutorial:
                if (_tutorialPanel != null) _tutorialPanel.SetActive(true);
                break;

            case GameState.Victory:
                if (_victoryPanel != null) _victoryPanel.SetActive(true);
                break;

            case GameState.Defeat:
                if (_defeatPanel != null) _defeatPanel.SetActive(true);
                break;
        }
    }

    private void HideAllPanels()
    {
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
        if (_levelSelectionPanel != null) _levelSelectionPanel.SetActive(false);
        if (_gameUIPanel != null) _gameUIPanel.SetActive(false);
        if (_pausePanel != null) _pausePanel.SetActive(false);
        if (_settingsPanel != null) _settingsPanel.SetActive(false);
        if (_tutorialPanel != null) _tutorialPanel.SetActive(false);
        if (_victoryPanel != null) _victoryPanel.SetActive(false);
        if (_defeatPanel != null) _defeatPanel.SetActive(false);
    }
}