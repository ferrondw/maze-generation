using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObjectBobbing : MonoBehaviour
{
    [SerializeField] private float height = 0.2f;
    [SerializeField] private float duration = 2f;
    private void Start()
    {
        var desiredPosition = transform.position + new Vector3(0, height, 0);
        if (height != 0)
        {
            transform.DOLocalMove(desiredPosition, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        transform.DOLocalRotate(new Vector3(0, 180, 0), duration).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }
}