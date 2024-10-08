using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FreePlayManager : MonoBehaviour
{
    public ushort width;
    public ushort height;

    public Slider widthSlider;
    public Slider heightSlider;
    public TMP_Text widthText;
    public TMP_Text heightText;
    public GameObject menu;
    
    [SerializeField] private Material transitionMaterial;
    private static readonly int Size = Shader.PropertyToID("_Size");

    private MazeRenderer mazeRenderer;
    private DecorationGenerator decorationGenerator;
    private Player player;

    public void Home()
    {
        transitionMaterial.DOFloat(0, Size, 0.7f).SetEase(Ease.InOutSine).OnComplete(() => { SceneManager.LoadScene(0); });
    }

    private void Start()
    {
        mazeRenderer = FindObjectOfType<MazeRenderer>();
        decorationGenerator = FindObjectOfType<DecorationGenerator>();
        player = FindObjectOfType<Player>();
        mazeRenderer.Clear();
        
        transitionMaterial.DOFloat(1, Size, 0.7f).SetEase(Ease.InOutSine);
        
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
        
        var (maze, path) = MazeGenerator.Generate(width, height, new RecursiveBacktrackingStrategy()); // FIX
        
        player.SetMaze(maze);
        mazeRenderer.Draw(maze);
        decorationGenerator.Generate(width, height, 20, 3);
    }
}
