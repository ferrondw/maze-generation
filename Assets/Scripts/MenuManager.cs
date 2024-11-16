using System;
using System.Globalization;
using DG.Tweening;
using TMPro;
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
    [SerializeField] private CanvasGroup menuBackdrop;
    [SerializeField] private CellExecute[] cellExecutes;
    [SerializeField] private Transition transition;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text acornAmountText;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicSliderText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxSliderText;

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
        player.Teleport(new Vector2Int(10, 10), true);

        transition.FromBlack();
        levelText.text = PlayerPrefs.GetInt("CurrentLevel", 1).ToString();
        acornAmountText.text = PlayerPrefs.GetInt("coins", 0).ToString();

        decorationGenerator.Generate(20, 20, 20, 0);
        InitialiseSettings();
    }

    private void InitialiseSettings()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.4f);
        musicSliderText.text = (Mathf.Round(musicSlider.value * 100) / 100).ToString(CultureInfo.CurrentCulture);
        sfxSliderText.text = (Mathf.Round(sfxSlider.value * 100) / 100).ToString(CultureInfo.CurrentCulture);
        AudioManager.Instance.UpdateSources(musicSlider.value, sfxSlider.value);
    }

    public void OnSliderValueChanged()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
        musicSliderText.text = (Mathf.Round(musicSlider.value * 100) / 100).ToString(CultureInfo.CurrentCulture);
        sfxSliderText.text = (Mathf.Round(sfxSlider.value * 100) / 100).ToString(CultureInfo.CurrentCulture);
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
        transition.ToBlack(0, () => { SceneManager.LoadScene(name); });
    }

    public void ClearALLData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void OpenMenu(int menuID)
    {
        menus[menuID].DOScale(1, 0.4f);
        menuBackdrop.DOFade(1, 0.3f);
    }

    public void CloseMenu(int menuID)
    {
        menus[menuID].DOScale(0.00001f, 0.4f); // flat zero values will do weird stuff with the scroll rect for some reason
        menuBackdrop.DOFade(0, 0.3f);
    }

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