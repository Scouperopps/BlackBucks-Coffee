using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance => _instance;

    private int _selectedLevelIndex = 0;
    public int SelectedLevelIndex => _selectedLevelIndex;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectAndStartLevel(int levelIndex)
    {
        _selectedLevelIndex = levelIndex;
        LevelSelection.SelectedLevel = levelIndex;

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.ResetLevelState();
            WaveManager.Instance.StartLevel(_selectedLevelIndex);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
}