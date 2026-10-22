
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera mainCamera;

    private void LateUpdate()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        transform.rotation = mainCamera.transform.rotation;
    }
}
