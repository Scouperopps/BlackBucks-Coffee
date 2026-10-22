using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReputationUI : MonoBehaviour
{
    [SerializeField] private Slider _reputationSlider;
    [SerializeField] private TMP_Text _reputationPercentageText;
    [SerializeField] private TMP_Text _endGameSummaryText;

    private void OnEnable()
    {
        if (ReputationManager.Instance != null)
        {
            ReputationManager.Instance.OnReputationChanged += UpdateReputationUI;
        }
    }

    private void OnDisable()
    {
        if (ReputationManager.Instance != null)
        {
            ReputationManager.Instance.OnReputationChanged -= UpdateReputationUI;
        }
    }

    private void UpdateReputationUI(float currentReputation, float maxReputation)
    {
        if (_reputationSlider != null)
        {
            _reputationSlider.value = currentReputation / maxReputation;
        }

        if (_reputationPercentageText != null)
        {
            _reputationPercentageText.text = $"{Mathf.RoundToInt((currentReputation / maxReputation) * 100f)}%";
        }
    }

    public void ShowEndGameResults()
    {
        if (_endGameSummaryText != null && ReputationManager.Instance != null)
        {
            _endGameSummaryText.text = ReputationManager.Instance.GetEndGameSummaryText();
        }
    }
}