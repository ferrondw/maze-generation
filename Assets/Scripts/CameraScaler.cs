using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float padding;

    public void ScaleCameraToMaze(int width, int height)
    {
        int max = Mathf.Max(width, height);
        cam.orthographicSize = max * 0.5f + padding;
    }
}
