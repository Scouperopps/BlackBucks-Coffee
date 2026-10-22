using System;
using UnityEngine;

public class ReputationManager : MonoBehaviour
{
    private static ReputationManager _instance;
    public static ReputationManager Instance => _instance;

    public float maxReputation = 100f;

    private float _currentReputation;
    public float CurrentReputation => _currentReputation;

    private int _totalScorePoints;
    public int TotalScorePoints => _totalScorePoints;

    private int _totalSatisfiedCustomers;
    public int TotalSatisfiedCustomers => _totalSatisfiedCustomers;

    private int _totalAngryCustomers;
    public int TotalAngryCustomers => _totalAngryCustomers;

    public event Action<float, float> OnReputationChanged;
    public event Action OnReputationDepleted;

    private const float VERY_HAPPY_THRESHOLD = 0.8f;
    private const float HAPPY_THRESHOLD = 0.6f;
    private const float NEUTRAL_THRESHOLD = 0.35f;
    private const float UNHAPPY_THRESHOLD = 0.05f;

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
        _currentReputation = maxReputation * 0.5f;
        OnReputationChanged?.Invoke(_currentReputation, maxReputation);
    }

    public CustomerMood CalculateCustomerMood(float remainingPatienceRatio)
    {
        if (remainingPatienceRatio >= VERY_HAPPY_THRESHOLD)
        {
            return CustomerMood.VeryHappy;
        }
        if (remainingPatienceRatio >= HAPPY_THRESHOLD)
        {
            return CustomerMood.Happy;
        }
        if (remainingPatienceRatio >= NEUTRAL_THRESHOLD)
        {
            return CustomerMood.Neutral;
        }
        if (remainingPatienceRatio >= UNHAPPY_THRESHOLD)
        {
            return CustomerMood.Unhappy;
        }
        return CustomerMood.Angry;
    }

    public void ProcessOrderDelivery(float remainingPatienceRatio)
    {
        CustomerMood customerMood = CalculateCustomerMood(remainingPatienceRatio);
        float reputationChangeAmount = 0f;
        int scorePointsGain = 0;

        switch (customerMood)
        {
            case CustomerMood.VeryHappy:
                reputationChangeAmount = 15f;
                scorePointsGain = 100;
                break;

            case CustomerMood.Happy:
                reputationChangeAmount = 10f;
                scorePointsGain = 75;
                break;

            case CustomerMood.Neutral:
                reputationChangeAmount = 5f;
                scorePointsGain = 50;
                break;

            case CustomerMood.Unhappy:
                reputationChangeAmount = 2f;
                scorePointsGain = 20;
                break;

            case CustomerMood.Angry:
                reputationChangeAmount = -15f;
                scorePointsGain = 0;
                break;
        }

        _totalScorePoints += scorePointsGain;

        if (customerMood != CustomerMood.Angry)
        {
            _totalSatisfiedCustomers++;
        }
        else
        {
            _totalAngryCustomers++;
        }

        ModifyReputation(reputationChangeAmount);
    }

    public void ProcessCustomerLeftAngry()
    {
        _totalAngryCustomers++;
        ModifyReputation(-20f);
    }

    private void ModifyReputation(float amount)
    {
        _currentReputation = Mathf.Clamp(_currentReputation + amount, 0f, maxReputation);
        OnReputationChanged?.Invoke(_currentReputation, maxReputation);

        if (_currentReputation <= 0f)
        {
            OnReputationDepleted?.Invoke();
        }
    }

    public string GetEndGameSummaryText()
    {
        return $"FINAL SCORE: {_totalScorePoints}\nREPUTATION: {Mathf.RoundToInt((_currentReputation / maxReputation) * 100f)}%\nCUSTOMERS SERVED: {_totalSatisfiedCustomers}\nCUSTOMERS LOST: {_totalAngryCustomers}";
    }
}