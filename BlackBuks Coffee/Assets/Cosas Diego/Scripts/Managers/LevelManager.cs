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

    public bool IsLevelUnlocked(int levelIndex)
    {
        if (SaveSystem.CurrentData == null) SaveSystem.LoadSlot(SaveSystem.ActiveSlot);
        if (levelIndex <= 1) return true;

        return levelIndex <= SaveSystem.CurrentData.highestUnlockedLevel;
    }

    public void SelectAndStartLevel(int levelIndex)
    {
        if (!IsLevelUnlocked(levelIndex))
        {
            Debug.LogWarning($"[LevelManager] Nivel {levelIndex} bloqueado.");
            return;
        }

        _selectedLevelIndex = levelIndex;

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