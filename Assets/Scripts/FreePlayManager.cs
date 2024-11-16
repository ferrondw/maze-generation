using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FreePlayManager : MonoBehaviour
{
    [SerializeField] private Slider widthSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private TMP_Text widthText;
    [SerializeField] private TMP_Text heightText;
    [SerializeField] private Transform menu;
    [SerializeField] private CanvasGroup menuBackdrop;
    [SerializeField] private Transition transition;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private MazeRenderer mazeRenderer;
    [SerializeField] private DecorationGenerator decorationGenerator;
    [SerializeField] private Player player;

    private ushort width;
    private ushort height;
    private bool isAnimating;

    public void Home()
    {
        transition.ToBlack(0, () => { SceneManager.LoadScene(0); });
    }

    private void Start()
    {
        mazeRenderer.Clear();
        isAnimating = false;
        OnValueChange();
        SetupLevel();
    }

    public void OnValueChange()
    {
        width = (ushort)widthSlider.value;
        height = (ushort)heightSlider.value;

        widthText.text = $"{width}";
        heightText.text = $"{height}";
    }

    public void ToggleMenu()
    {
        if (isAnimating) return;
        isAnimating = true;

        var endValue = menu.localScale == Vector3.zero ? 1 : 0;

        player.canMove = endValue == 0;
        menu.DOScale(endValue, 0.4f).OnComplete(() => { isAnimating = false; });
        menuBackdrop.DOFade(endValue, 0.3f);
    }

    public void OpenMenu()
    {
        if (isAnimating) return;
        isAnimating = true;
        
        player.canMove = false;
        menu.DOScale(1, 0.4f).OnComplete(() => { isAnimating = false; });
        menuBackdrop.DOFade(1, 0.3f);
    }

    public void Generate()
    {
        transition.ToBlack(0, SetupLevel);
    }

    private void SetupLevel()
    {
        mazeRenderer.Clear();
        menu.DOScale(0, 0.4f);
        menuBackdrop.DOFade(0, 0.3f);
        player.canMove = true;

        var (maze, path) = MazeGenerator.Generate(width, height, new RecursiveBacktrackingStrategy());

        player.SetMaze(maze);
        cameraFollow.SnapToTarget();
        mazeRenderer.Draw(maze);
        decorationGenerator.Generate(width, height, 20, 3);

        transition.FromBlack(0.1f);
    }
}