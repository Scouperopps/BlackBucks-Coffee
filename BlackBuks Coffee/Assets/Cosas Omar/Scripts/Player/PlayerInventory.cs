using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Transform sphereHolder;

    [SerializeField] private GameObject redSpherePrefab;
    [SerializeField] private GameObject blueSpherePrefab;
    [SerializeField] private GameObject greenSpherePrefab;

    private List<SphereColor> currentSpheres = new List<SphereColor>();

    private List<GameObject> currentSphereObjects = new List<GameObject>();

    public bool HasSphere()
    {
        return currentSpheres.Count > 0;
    }

    public List<SphereColor> GetSpheres()
    {
        return currentSpheres;
    }

    public bool TryAddSphere(SphereColor color)
    {
        currentSpheres.Add(color);

        CreateSphereVisual(color);

        Debug.Log("Has recogido una esfera " + color);

        return true;
    }

    public void RemoveAllSpheres()
    {
        if (currentSpheres.Count == 0)
            return;

        currentSpheres.Clear();

        DestroySphereVisuals();

        Debug.Log("Has entregado todas las esferas.");
    }

    private void CreateSphereVisual(SphereColor color)
    {
        GameObject prefab = GetSpherePrefab(color);

        if (prefab == null)
        {
            Debug.LogError("No hay un prefab asignado para " + color);
            return;
        }

        GameObject sphereObject = Instantiate(
            prefab,
            sphereHolder
        );

        sphereObject.transform.localPosition =
            Vector3.right * currentSphereObjects.Count * 0.5f;

        sphereObject.transform.localRotation = Quaternion.identity;

        currentSphereObjects.Add(sphereObject);
    }

    private void DestroySphereVisuals()
    {
        foreach (GameObject sphereObject in currentSphereObjects)
        {
            if (sphereObject != null)
            {
                Destroy(sphereObject);
            }
        }

        currentSphereObjects.Clear();
    }

    private GameObject GetSpherePrefab(SphereColor color)
    {
        switch (color)
        {
            case SphereColor.Red:
                return redSpherePrefab;

            case SphereColor.Blue:
                return blueSpherePrefab;

            case SphereColor.Green:
                return greenSpherePrefab;

            default:
                return null;
        }
    }
}