using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Offset")]
    [SerializeField] private Vector3 offset = new Vector3(-3.5f, 10f, -3.5f);

    [Header("Follow")]
    [SerializeField] private float followSpeed = 8f;

    [Header("Camera Limits")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    [SerializeField] private float minZ = -10f;
    [SerializeField] private float maxZ = 10f;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(61f, 45f, 0f);
    }

    private void LateUpdate()
    {
        if (target == null)return;
        
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);
        transform.position = Vector3.Lerp(transform.position,desiredPosition,followSpeed * Time.deltaTime);
    }
}