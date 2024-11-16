using DG.Tweening;
using TMPro;
using UnityEngine;

public class AcornCollect : MonoBehaviour
{
    private const int acornValue = 1;

    public void Collect(TMP_Text acornAmountText)
    {
        AudioManager.Instance.PlayClip(1, Random.Range(0, 0.3f));
        transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(gameObject);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + acornValue);
            acornAmountText.text = PlayerPrefs.GetInt("coins", 0).ToString();
        });
    }
}