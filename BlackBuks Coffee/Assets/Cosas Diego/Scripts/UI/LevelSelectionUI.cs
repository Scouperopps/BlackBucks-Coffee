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

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < _levelButtons.Length; i++)
        {
            int index = _levelButtons[i].levelIndex;
            if (_levelButtons[i].levelButton != null)
            {
                _levelButtons[i].levelButton.onClick.RemoveAllListeners();
                _levelButtons[i].levelButton.onClick.AddListener(() => OnLevelSelected(index));
            }
        }
    }

    private void OnLevelSelected(int levelIndex)
    {
        LevelManager.Instance.SelectAndStartLevel(levelIndex);
    }
}