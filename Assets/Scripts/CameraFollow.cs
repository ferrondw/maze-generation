using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float smoothTime;
    [SerializeField] private Transform target;
    
    private Vector3 velocity = Vector3.zero;

    public void SnapToTarget() => transform.position = target.position;
    
    private void Update()
    {
        var desiredPosition = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
        transform.position = desiredPosition;
    }

}