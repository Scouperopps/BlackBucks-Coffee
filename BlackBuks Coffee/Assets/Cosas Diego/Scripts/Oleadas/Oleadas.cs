using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public string waveName = "Wave";
    public int customerCount = 5;
    public float timeBetweenSpawns = 8f;   // segundos entre cada cliente
}

[System.Serializable]
public class LevelData
{
    public string levelName = "Nivel";
    public float shiftDuration = 120f;     // para el GameManager cuando lo tengas
    public float customerPatience = 15f;   // se le pasa a cada cliente al crearlo
    public int maxItemsPerOrder = 1;       // cada cliente pide entre 1 y este número de esferas
    public List<WaveData> waves = new List<WaveData>();
}

// El menú asigna esto antes de cargar la escena (0 = nivel 1)
public static class LevelSelection
{
    public static int SelectedLevel = 0;
}

public class WaveManager : MonoBehaviour
{
    [Header("Cliente")]
    [SerializeField] private CustomerTesting customerPrefab;

    [Header("Puntos de la escena")]
    [SerializeField] private Transform spawnPoint;      // la puerta
    [SerializeField] private Transform customerPoint;   // donde espera su pedido
    [SerializeField] private Transform exitPoint;       // por donde se va

    [Header("Niveles (cada uno con sus oleadas)")]
    public List<LevelData> levels = new List<LevelData>();

    [Header("Configuración General")]
    [SerializeField] private bool autoStart = true;     // desmárcalo si el menú está en la misma escena
    public float timeBetweenWaves = 3f;
    [Range(0f, 0.5f)] public float spawnJitter = 0.2f;  // variación aleatoria del intervalo

    private Coroutine levelRoutine;
    private int activeCustomers = 0;

    public int ActiveCustomers => activeCustomers;

    public System.Action<int> OnWaveStarted;            // índice de la oleada
    public System.Action OnAllWavesCompleted;
    public System.Action<bool> OnCustomerResolved;      // true = satisfecho, false = se fue molesto

    private void Start()
    {
        if (autoStart)
            StartLevel(LevelSelection.SelectedLevel);
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogError($"[WaveManager] Nivel inválido: {levelIndex}");
            return;
        }

        if (levelRoutine != null)
            StopCoroutine(levelRoutine);

        levelRoutine = StartCoroutine(RunLevel(levels[levelIndex]));
    }

    private IEnumerator RunLevel(LevelData level)
    {
        for (int i = 0; i < level.waves.Count; i++)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            OnWaveStarted?.Invoke(i);
            yield return StartCoroutine(SpawnWave(level.waves[i], level.customerPatience, level.maxItemsPerOrder));
        }

        Debug.Log("[WaveManager] Todas las oleadas completadas.");
        OnAllWavesCompleted?.Invoke();
    }

    private IEnumerator SpawnWave(WaveData wave, float patience, int maxItems)
    {
        for (int i = 0; i < wave.customerCount; i++)
        {
            SpawnCustomer(patience, maxItems);

            float jitter = Random.Range(1f - spawnJitter, 1f + spawnJitter);
            yield return new WaitForSeconds(wave.timeBetweenSpawns * jitter);
        }
    }

    private void SpawnCustomer(float patience, int maxItems)
    {
        if (customerPrefab == null || spawnPoint == null)
        {
            Debug.LogError("[WaveManager] Falta asignar el prefab del cliente o el spawnPoint.");
            return;
        }

        CustomerTesting customer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        customer.Init(customerPoint, exitPoint, patience, maxItems);
        customer.OnCustomerLeft += HandleCustomerLeft;
        activeCustomers++;
    }

    private void HandleCustomerLeft(CustomerTesting customer, bool satisfied)
    {
        customer.OnCustomerLeft -= HandleCustomerLeft;
        activeCustomers--;
        OnCustomerResolved?.Invoke(satisfied);
    }
}