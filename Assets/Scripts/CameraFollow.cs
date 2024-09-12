using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothTime;
    public Transform target;
    
    private Vector3 velocity = Vector3.zero;
    
    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
    }
}
