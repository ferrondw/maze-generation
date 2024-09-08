using System;
using System.IO;
using System.Text;
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
    private Cell[,] maze;

    void OnGUI()
    {
        width = (byte)EditorGUILayout.IntSlider("Width", width, 4, 255);
        height = (byte)EditorGUILayout.IntSlider("Height", height, 4, 255);
        seed = EditorGUILayout.IntField("Seed (-1 for random)", seed);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
        {
            MazeRenderer renderer = FindObjectOfType<MazeRenderer>();
            renderer.Clear();
        }
        if (GUILayout.Button("Generate"))
        {
            MazeRenderer renderer = FindObjectOfType<MazeRenderer>();
            renderer.Clear();
            
            maze = MazeGenerator.Generate(width, height, seed);
            renderer.Draw(maze, width, height);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Export"))
        {
            var bytes = MazeGenerator.MazeToBytes(maze, width, height);

            string path = EditorUtility.SaveFilePanel("Save Maze", Environment.SpecialFolder.Personal + "/Downloads/", "export", "maze");
            if (path.Length != 0)
            {
                if (bytes != null)
                {
                    File.WriteAllBytes(path, bytes);
                    AssetDatabase.Refresh();
                }
            }
        }
        if (GUILayout.Button("Import"))
        {
            string path = EditorUtility.OpenFilePanel("Import maze", "", "maze");
            if (path.Length != 0)
            {
                var bytes = File.ReadAllBytes(path);
                var maze = MazeGenerator.BytesToMaze(bytes);
                MazeRenderer renderer = FindObjectOfType<MazeRenderer>();
                renderer.Clear();
                renderer.Draw(maze, Convert.ToUInt16(bytes[0]), Convert.ToUInt16(bytes[1]));
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}