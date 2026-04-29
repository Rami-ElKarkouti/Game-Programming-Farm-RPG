using UnityEngine;
    

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Map Bounds")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [Header("Follow")]
    [SerializeField] private float smoothSpeed = 5f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

   private void LateUpdate()
{
    if (target == null) return;

    float halfHeight = cam.orthographicSize;
    float halfWidth = cam.orthographicSize * cam.aspect;

    float minCamX = minX + halfWidth;
    float maxCamX = maxX - halfWidth;
    float minCamY = minY + halfHeight;
    float maxCamY = maxY - halfHeight;

    Debug.Log($"target={target.position} halfW={halfWidth} halfH={halfHeight} camX range=({minCamX},{maxCamX}) camY range=({minCamY},{maxCamY})");

    float clampedX = Mathf.Clamp(target.position.x, minCamX, maxCamX);
    float clampedY = Mathf.Clamp(target.position.y, minCamY, maxCamY);

    Vector3 desiredPosition = new Vector3(clampedX, clampedY, transform.position.z);
    transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
}
}

