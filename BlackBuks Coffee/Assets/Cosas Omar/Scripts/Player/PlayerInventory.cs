using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Capacidad e Inventario")]
    [SerializeField] private int maxCapacity = 3;
    [SerializeField] private float stackOffset = 0.4f; 

    [Header("Visuales")]
    [SerializeField] private Transform sphereHolder;
    [SerializeField] private GameObject redSpherePrefab;
    [SerializeField] private GameObject blueSpherePrefab;
    [SerializeField] private GameObject greenSpherePrefab;

    private readonly List<SphereColor> carriedSpheres = new List<SphereColor>();
    private readonly List<GameObject> spawnedVisuals = new List<GameObject>();

    public bool HasSphere() => carriedSpheres.Count > 0;
    public bool IsFull() => carriedSpheres.Count >= maxCapacity;
    public IReadOnlyList<SphereColor> CarriedSpheres => carriedSpheres;

    public bool TryAddSphere(SphereColor color)
    {
        if (IsFull())
        {
            Debug.Log("Inventario lleno. No puedes llevar más esferas.");
            return false;
        }

        carriedSpheres.Add(color);
        CreateSphereVisual(color);
        Debug.Log($"Has recogido una esfera {color}. Llevas ({carriedSpheres.Count}/{maxCapacity})");
        return true;
    }

    public bool TryRemoveSphere(SphereColor color)
    {
        int index = carriedSpheres.IndexOf(color);
        if (index < 0) return false;

        carriedSpheres.RemoveAt(index);
        DestroySphereVisualAt(index);
        RealignVisuals();
        Debug.Log($"Has entregado la esfera {color}");
        return true;
    }

    private void CreateSphereVisual(SphereColor color)
    {
        GameObject prefab = GetSpherePrefab(color);
        if (prefab == null || sphereHolder == null) return;

        GameObject visual = Instantiate(prefab, sphereHolder);
        visual.transform.localPosition = Vector3.up * (spawnedVisuals.Count * stackOffset);
        visual.transform.localRotation = Quaternion.identity;
        spawnedVisuals.Add(visual);
    }

    private void DestroySphereVisualAt(int index)
    {
        if (index >= 0 && index < spawnedVisuals.Count)
        {
            Destroy(spawnedVisuals[index]);
            spawnedVisuals.RemoveAt(index);
        }
    }

    private void RealignVisuals()
    {
        for (int i = 0; i < spawnedVisuals.Count; i++)
        {
            if (spawnedVisuals[i] != null)
            {
                spawnedVisuals[i].transform.localPosition = Vector3.up * (i * stackOffset);
            }
        }
    }

    private GameObject GetSpherePrefab(SphereColor color)
    {
        switch (color)
        {
            case SphereColor.Red: return redSpherePrefab;
            case SphereColor.Blue: return blueSpherePrefab;
            case SphereColor.Green: return greenSpherePrefab;
            default: return null;
        }
    }
}