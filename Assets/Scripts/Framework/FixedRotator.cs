using UnityEngine;

public class FixedRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 10, 0);

    private void Update()
    {
        transform.localRotation *= Quaternion.Euler(rotationSpeed * Time.deltaTime);
    }
}
