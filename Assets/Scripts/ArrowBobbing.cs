using DG.Tweening;
using UnityEngine;

public class ArrowBobbing : MonoBehaviour
{
    [SerializeField] private Vector3 direction;

    private void Start()
    {
        transform.DOLocalMove(transform.localPosition + direction, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}