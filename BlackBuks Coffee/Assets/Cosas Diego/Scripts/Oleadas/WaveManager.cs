using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public string waveName = "Wave";
    public int customerCount = 5;
    public float timeBetweenSpawns = 8f;
}

[System.Serializable]
public class LevelData
{
    public string levelName = "Level";
    public float shiftDuration = 120f;
    public float customerPatience = 15f;
    public int maxItemsPerOrder = 1;
    public List<WaveData> waves = new List<WaveData>();
}

public static class LevelSelection
{
    public static int SelectedLevel = 0;
}

public class WaveManager : MonoBehaviour
{
    [SerializeField] private CustomerTesting _customerPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _customerPoint;
    [SerializeField] private Transform _exitPoint;

    public List<LevelData> levels = new List<LevelData>();
    public float timeBetweenWaves = 3f;
    [Range(0f, 0.5f)] public float spawnJitter = 0.2f;

    private Coroutine _levelRoutine;
    private int _activeCustomers = 0;
    private bool _levelStarted = false;

    public int ActiveCustomers => _activeCustomers;

    public System.Action<int> OnWaveStarted;
    public System.Action OnAllWavesCompleted;
    public System.Action<bool> OnCustomerResolved;

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            return;

        if (!_levelStarted)
        {
            _levelStarted = true;
            StartLevel(LevelSelection.SelectedLevel);
        }
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
            return;

        if (_levelRoutine != null)
            StopCoroutine(_levelRoutine);

        _levelRoutine = StartCoroutine(RunLevel(levels[levelIndex]));
    }

    private IEnumerator RunLevel(LevelData level)
    {
        for (int i = 0; i < level.waves.Count; i++)
        {
            yield return StartCoroutine(WaitForSecondsPaused(timeBetweenWaves));
            OnWaveStarted?.Invoke(i);
            yield return StartCoroutine(SpawnWave(level.waves[i], level.customerPatience, level.maxItemsPerOrder));
        }

        OnAllWavesCompleted?.Invoke();
    }

    private IEnumerator SpawnWave(WaveData wave, float patience, int maxItems)
    {
        for (int i = 0; i < wave.customerCount; i++)
        {
            while (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            {
                yield return null;
            }

            SpawnCustomer(patience, maxItems);

            float jitter = Random.Range(1f - spawnJitter, 1f + spawnJitter);
            yield return StartCoroutine(WaitForSecondsPaused(wave.timeBetweenSpawns * jitter));
        }
    }

    private IEnumerator WaitForSecondsPaused(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPlaying)
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }
    }

    private void SpawnCustomer(float patience, int maxItems)
    {
        if (_customerPrefab == null || _spawnPoint == null)
            return;

        CustomerTesting customer = Instantiate(_customerPrefab, _spawnPoint.position, _spawnPoint.rotation);
        customer.Init(_customerPoint, _exitPoint, patience, maxItems);

        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.AddToQueue(customer);
        }

        customer.OnCustomerLeft += HandleCustomerLeft;
        _activeCustomers++;
    }

    private void HandleCustomerLeft(CustomerTesting customer, bool satisfied)
    {
        customer.OnCustomerLeft -= HandleCustomerLeft;
        _activeCustomers--;
        OnCustomerResolved?.Invoke(satisfied);
    }
}