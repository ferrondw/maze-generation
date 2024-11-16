using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private MazeRenderer mazeRenderer;
    [SerializeField] private DecorationGenerator decorationGenerator;
    [SerializeField] private Player player;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private AcornManager acornManager;
    [SerializeField] private Transition transition;
    [SerializeField] private Transform menu;
    [SerializeField] private CanvasGroup menuBackdrop;
    [SerializeField] private TMP_Text menuStatsText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text acornAmountText;

    private static int currentLevel = 1;
    private LevelData currentLevelData;
    private float startTime;
    private bool isLevelActive;
    private List<Acorn> acorns;

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        player.onPositionUpdate.AddListener(OnAcornPos);
        SetupLevel();
    }
    
    private void Update()
    {
        if (isLevelActive) UpdateTimerText();
    }

    public void BeginLevel(int overrideLevel = -1)
    {
        transition.ToBlack(0, () => { SetupLevel(); }); // cant use method group because of the parameter
    }

    private void SetupLevel(int overrideLevel = -1)
    {
        menu.DOScale(0, 0);
        menuBackdrop.DOFade(0, 0);
        acornManager.Clear();
        Generate(overrideLevel == -1 ? currentLevel : overrideLevel);
        acornAmountText.text = PlayerPrefs.GetInt("coins", 0).ToString();
        UpdateLevelText();
        startTime = Time.time; // hacky fix, but make sure the time is flat 0 when transitioning
        UpdateTimerText();
        
        transition.FromBlack(0.1f, () =>
        {
            startTime = Time.time;
            player.canMove = true;
            isLevelActive = true;
        });
    }

    public void EndLevel()
    {
        var elapsedTime = Time.time - startTime;
        var stepsTaken = player.positionHistory.Count;
        currentLevelData.UpdateCompletionStats(elapsedTime, stepsTaken);
        SaveLevelData(currentLevel, currentLevelData);

        player.canMove = false;
        isLevelActive = false;
        ShowEndScreen(currentLevel, elapsedTime, stepsTaken);

        if (currentLevel != PlayerPrefs.GetInt("CurrentLevel", 1)) return;

        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
    }

    public void Home()
    {
        transition.ToBlack(0, () => { SceneManager.LoadScene(0); });
    }

    private void ShowEndScreen(int levelIndex, float time, int steps)
    {
        menu.DOScale(1, 0.4f);
        menuBackdrop.DOFade(1, 0.3f);
        var size = GetLevelDimensions(levelIndex);
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        menuStatsText.text = $"Level: {levelIndex}\nSize: {size}x{size}\nTime: {timeSpan:mm\\:ss\\.fff}\nSteps: {steps}";
    }
    
    private void UpdateLevelText()
    {
        var size = GetLevelDimensions(currentLevel);
        levelText.text = $"Level {currentLevel} ({size}x{size})";
    }
    
    private void UpdateTimerText()
    {
        float elapsedTime = Time.time - startTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
        string formattedTime = $@"{timeSpan:mm\:ss\.fff}";
        timeText.text = formattedTime;
    }

    private void Generate(int index)
    {
        var levelDat = LoadLevelData(index);
        SaveLevelData(index, levelDat);
        var (maze, path) = MazeGenerator.Generate(levelDat.width, levelDat.height, new RecursiveBacktrackingStrategy(), levelDat.seed);

        mazeRenderer.Clear();
        player.SetMaze(maze);
        cameraFollow.SnapToTarget();
        mazeRenderer.Draw(maze);
        
        acorns = acornManager.Spawn((ushort)maze.GetLength(0), (ushort)maze.GetLength(1));
        decorationGenerator.Generate(levelDat.width, levelDat.height, 20, 3);
    }

    private void OnAcornPos(Vector2Int playerPosition)
    {
        for (int i = acorns.Count - 1; i >= 0; i--)
        {
            if (acorns[i].position != playerPosition) continue;
            
            acorns[i].gameObject.GetComponent<AcornCollect>().Collect(acornAmountText);
            acorns.RemoveAt(i);
                
            break;
        }
    }


    private static void SaveLevelData(int index, LevelData levelData)
    {
        string json = JsonConvert.SerializeObject(levelData);
        PlayerPrefs.SetString("Level_" + index, json);
        PlayerPrefs.Save();
    }

    private static LevelData LoadLevelData(int index)
    {
        string key = "Level_" + index;
        if (PlayerPrefs.HasKey(key)) return JsonConvert.DeserializeObject<LevelData>(PlayerPrefs.GetString(key));

        var seed = (ushort)Random.Range(0, ushort.MaxValue);
        var size = GetLevelDimensions(index);
        return new LevelData(size, size, seed);
    }

    private static ushort GetLevelDimensions(int level)
    {
        const int baseSize = 20;
        var increase = (level - 1) / 5;

        ushort size = (ushort)Math.Min(baseSize + increase, 100);

        return size;
    }
}