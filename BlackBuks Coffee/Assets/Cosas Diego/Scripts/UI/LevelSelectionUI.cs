using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionUI : MonoBehaviour
{
    [System.Serializable]
    public struct LevelButtonData
    {
        public int levelIndex;
        public Button levelButton;
    }

    [SerializeField] private LevelButtonData[] _levelButtons;

    private void OnEnable()
    {
        RefreshUI();
    }

    private void Start()
    {
        SetupButtons();
        RefreshUI();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < _levelButtons.Length; i++)
        {
            if (_levelButtons[i].levelButton != null)
            {
                int cachedIndex = _levelButtons[i].levelIndex;
                _levelButtons[i].levelButton.onClick.RemoveAllListeners();
                _levelButtons[i].levelButton.onClick.AddListener(() => OnLevelSelected(cachedIndex));
            }
        }
    }

    public void RefreshUI()
    {
        if (_levelButtons == null) return;

        for (int i = 0; i < _levelButtons.Length; i++)
        {
            if (_levelButtons[i].levelButton != null)
            {
                int index = _levelButtons[i].levelIndex;
                bool isUnlocked = LevelManager.Instance != null && LevelManager.Instance.IsLevelUnlocked(index);
                _levelButtons[i].levelButton.interactable = isUnlocked;
            }
        }
    }

    private void OnLevelSelected(int levelIndex)
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SelectAndStartLevel(levelIndex);
        }
    }
}