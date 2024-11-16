using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextAlphaPulse : MonoBehaviour
{
    [SerializeField] private float targetAlpha;
    [SerializeField] private float duration;

    private void Start()
    {
        GetComponent<TMP_Text>().DOFade(targetAlpha, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
