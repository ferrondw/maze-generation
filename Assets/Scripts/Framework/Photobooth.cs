#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

public class Photobooth : MonoBehaviour
{
    private const int gifFrameCount = 36 * 4;

    [MenuItem("Assets/Create Picture and GIF", true)]
    private static bool ValidateCreatePicture()
    {
        return Selection.activeObject is GameObject;
    }

    [MenuItem("Assets/Create Picture and GIF")]
    private static void CreatePictureAndGif()
    {
        var selectedPrefab = Selection.activeObject as GameObject;
        if (selectedPrefab == null) return;

        var instance = PrefabUtility.InstantiatePrefab(selectedPrefab) as GameObject;
        instance.transform.position = new Vector3(99999, 99999, 99999);

        var bounds = GetObjectBounds(instance);
        var camera = SetupCamera(bounds);

        string picturesDir = Path.Combine(Application.dataPath, "Pictures");
        if (!Directory.Exists(picturesDir))
        {
            Directory.CreateDirectory(picturesDir);
        }

        string frontImagePath = Path.Combine(picturesDir, selectedPrefab.name + "_front.png");
        SaveRenderTextureAsPng(camera, frontImagePath);

        var framePaths = CaptureGifFrames(camera, instance, bounds, selectedPrefab.name, picturesDir);

        string gifPath = Path.Combine(picturesDir, selectedPrefab.name + "_rotate.gif");
        GenerateGifFromFrames(framePaths, gifPath);

        DestroyImmediate(camera.gameObject);
        DestroyImmediate(instance);
        AssetDatabase.Refresh();
    }

    private static Bounds GetObjectBounds(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(obj.transform.position, Vector3.zero);
        }

        var bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }

    private static Camera SetupCamera(Bounds bounds)
    {
        var camera = new GameObject("TempCamera").AddComponent<Camera>();
        camera.transform.position = bounds.center - Vector3.forward * bounds.size.magnitude;
        camera.transform.LookAt(bounds.center);
        camera.orthographic = true;
        camera.orthographicSize = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0, 0, 0, 0);
        return camera;
    }

    private static void SaveRenderTextureAsPng(Camera camera, string filePath)
    {
        var renderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
        camera.targetTexture = renderTexture;
        camera.Render();

        RenderTexture.active = renderTexture;
        var texture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        texture.Apply();

        var bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        UnityEngine.Debug.Log("Picture saved to: " + filePath);

        RenderTexture.active = null;
        camera.targetTexture = null;
        DestroyImmediate(renderTexture);
    }

    private static List<string> CaptureGifFrames(Camera camera, GameObject instance, Bounds bounds, string prefabName, string directory)
    {
        var framePaths = new List<string>();

        for (int i = 0; i < gifFrameCount; i++)
        {
            instance.transform.RotateAround(bounds.center, Vector3.up, 360f / gifFrameCount);

            RenderTexture renderTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
            texture.Apply();

            string framePath = Path.Combine(directory, $"{prefabName}_frame_{i:D3}.png");
            File.WriteAllBytes(framePath, texture.EncodeToPNG());
            framePaths.Add(framePath);

            RenderTexture.active = null;
            camera.targetTexture = null;
            DestroyImmediate(renderTexture);
        }

        return framePaths;
    }

    private static void GenerateGifFromFrames(List<string> framePaths, string outputGifPath)
    {
        string directory = Path.GetDirectoryName(framePaths[0]);

        if (directory != null)
        {
            string framePattern = Path.Combine(directory, Path.GetFileNameWithoutExtension(framePaths[0]).Replace("_000", "_%03d") + ".png");

            string ffmpegCommand = $"ffmpeg -y -i \"{framePattern}\" -vf \"fps=30,scale=512:512:flags=lanczos\" -loop 0 \"{outputGifPath}\"";

            RunCommand(ffmpegCommand);
        }

        UnityEngine.Debug.Log($"GIF created at: {outputGifPath}");

        foreach (var frame in framePaths.Where(File.Exists))
        {
            File.Delete(frame);
            UnityEngine.Debug.Log($"Deleted temporary frame: {frame}");
        }
    }


    private static void RunCommand(string command)
    {
        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        Process process = new Process { StartInfo = processInfo };
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(output))
        {
            UnityEngine.Debug.Log("Command Output: " + output);
        }

        if (!string.IsNullOrEmpty(error))
        {
            UnityEngine.Debug.LogError("Command Error: " + error);
        }
    }
}
#endif