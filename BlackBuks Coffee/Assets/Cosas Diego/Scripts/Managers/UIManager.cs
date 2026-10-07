using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject mainMenuPanel;
    public GameObject playingPanel;
    public GameObject pausePanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;

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

    public void DisplayPanelByState(GameState state)
    {
        DisableAllPanels();

        switch (state)
        {
            case GameState.MainMenu:
                if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
                break;
            case GameState.Playing:
                if (playingPanel != null) playingPanel.SetActive(true);
                break;
            case GameState.Paused:
                if (pausePanel != null) pausePanel.SetActive(true);
                break;
            case GameState.Victory:
                if (victoryPanel != null) victoryPanel.SetActive(true);
                break;
            case GameState.Defeat:
                if (defeatPanel != null) defeatPanel.SetActive(true);
                break;
        }
    }

    private void DisableAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (playingPanel != null) playingPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
    }

    public void ClickPlayButton()
    {
        GameManager.Instance.StartGame();
    }

    public void ClickResumeButton()
    {
        GameManager.Instance.ResumeGame();
    }

    public void ClickRestartButton()
    {
        GameManager.Instance.RestartGame();
    }

    public void ClickMainMenuButton()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}