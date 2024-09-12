using Unity.Mathematics;
using UnityEngine;

public class ZeroRotation : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = quaternion.identity;
    }
}
