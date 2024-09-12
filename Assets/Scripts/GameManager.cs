using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public ushort width;
    public ushort height;

    public Slider widthSlider;
    public Slider heightSlider;
    public TMP_Text widthText;
    public TMP_Text heightText;
    public GameObject menu;

    private MazeRenderer mazeRenderer;
    private Player player;

    private void Start()
    {
        mazeRenderer = FindObjectOfType<MazeRenderer>();
        player = FindObjectOfType<Player>();
        mazeRenderer.Clear();
        
        onValueChange();
    }

    public void onValueChange()
    {
        width = (ushort)widthSlider.value;
        height = (ushort)heightSlider.value;

        widthText.text = $"Width: {width}";
        heightText.text = $"Height: {height}";
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }

    public void Generate()
    {
        mazeRenderer.Clear();
        
        var maze = MazeGenerator.Generate(width, height);
        
        player.currentMaze = maze;
        player.currentMazeSize = new Vector2Int(width, height);
        player.Reset();
        
        mazeRenderer.Draw(maze, width, height);
    }
}
