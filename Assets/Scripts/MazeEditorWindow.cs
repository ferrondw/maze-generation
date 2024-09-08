using UnityEditor;
using UnityEngine;

public class MazeEditorWindow : EditorWindow
{
    [MenuItem("Maze Generation/Generator")]
    public static void ShowWindow()
    {
        GetWindow<MazeEditorWindow>(false, "Maze Generator", true);
    }

    private byte width = 4;
    private byte height = 4;
    private int seed = 4;

    void OnGUI()
    {
        width = (byte)EditorGUILayout.IntSlider("Width", width, 4, 255);
        height = (byte)EditorGUILayout.IntSlider("Height", height, 4, 255);
        seed = EditorGUILayout.IntField("Seed (-1 for random)", seed);
        if (GUILayout.Button("Generate"))
        {
            MazeRenderer renderer = FindObjectOfType<MazeRenderer>();
            renderer.Clear();
            
            var maze = MazeGenerator.Generate(width, height, seed);
            renderer.Draw(maze, width, height);
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Export"))
        {
            Debug.Log("Exported!");
        }
        if (GUILayout.Button("Import"))
        {
            Debug.Log("Imported!");
        }
        EditorGUILayout.EndHorizontal();
    }
}