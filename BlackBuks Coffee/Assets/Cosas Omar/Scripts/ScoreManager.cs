using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private float _globalScore = 0f;
    public float GlobalScore => _globalScore;

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

    public void AddScore(float points)
    {
        _globalScore += points;
        Debug.Log($"Sumatoria global actualizada: {_globalScore} puntos.");
    }
}