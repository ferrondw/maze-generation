using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class CellExecute
{
    public Vector2Int pos;
    public UnityEvent toInvoke;
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private DecorationGenerator decorationGenerator;
    [SerializeField] private Transform[] menus;
    [SerializeField] private CellExecute[] cellExecutes;

    [SerializeField] private Material transitionMaterial;
    private static readonly int Size = Shader.PropertyToID("_Size");

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        var start = new Cell[20, 20];

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                start[i, j] = Cell.None;
            }
        }

        player.canMove = true;
        player.SetMaze(start);
        player.Teleport(new Vector2Int(10, 10));
        
        transitionMaterial.DOFloat(1, Size, 0.7f).SetEase(Ease.InOutSine);

        decorationGenerator.Generate(20, 20, 20, 0);
        InitialiseSettings();
    }

    private void InitialiseSettings()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.4f);
        AudioManager.Instance.UpdateSources(musicSlider.value, sfxSlider.value);
    }

    public void OnSliderValueChanged()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
        AudioManager.Instance.UpdateSources(musicSlider.value, sfxSlider.value);
    }

    public void OnPlayerPositionUpdate(Vector2Int pos)
    {
        foreach (var cellExecute in cellExecutes)
        {
            if (cellExecute.pos == pos)
            {
                cellExecute.toInvoke.Invoke();
            }
        }
    }

    public void LoadScene(string name)
    {
        player.canMove = false;
        transitionMaterial.DOFloat(0, Size, 0.7f).SetEase(Ease.InOutSine).OnComplete(() => { SceneManager.LoadScene(name); });
    }

    public void OpenMenu(int menuID) => menus[menuID].DOScale(1, 0.4f);
    public void CloseMenu(int menuID) => menus[menuID].DOScale(0, 0.4f);

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("finish");
            //if that doesn't work
            Application.Quit();
#endif
    }
}