using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Transform sphereHolder;

    [SerializeField] private GameObject redSpherePrefab;
    [SerializeField] private GameObject blueSpherePrefab;
    [SerializeField] private GameObject greenSpherePrefab;

    private SphereColor? currentSphere;
    private GameObject currentSphereObject;

    public bool HasSphere()
    {
        return currentSphere.HasValue;
    }

    public SphereColor GetSphereColor()
    {
        return currentSphere.Value;
    }

    public bool TryAddSphere(SphereColor color)
    {
        if (HasSphere())
        {
            Debug.Log("Ya tienes una esfera.");
            return false;
        }

        currentSphere = color;

        CreateSphereVisual(color);

        Debug.Log("Has recogido una esfera " + color);

        return true;
    }

    public void RemoveSphere()
    {
        if (!HasSphere())
            return;

        Debug.Log("Has entregado la esfera " + currentSphere.Value);

        currentSphere = null;

        DestroySphereVisual();
    }

    private void CreateSphereVisual(SphereColor color)
    {
        GameObject prefab = GetSpherePrefab(color);

        if (prefab == null)
        {
            Debug.LogError("No hay un prefab asignado para " + color);
            return;
        }

        currentSphereObject = Instantiate(
            prefab,
            sphereHolder.position,
            sphereHolder.rotation,
            sphereHolder
        );

        currentSphereObject.transform.localPosition = Vector3.zero;
        currentSphereObject.transform.localRotation = Quaternion.identity;
    }

    private void DestroySphereVisual()
    {
        if (currentSphereObject != null)
        {
            Destroy(currentSphereObject);
            currentSphereObject = null;
        }
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