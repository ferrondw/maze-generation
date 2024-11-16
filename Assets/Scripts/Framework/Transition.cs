using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Transition : MonoBehaviour
{
    [SerializeField] private Material transitionMaterial;
    private static readonly int _size = Shader.PropertyToID("_Size");

    public void FromBlack(float delay = 0f, Action onComplete = null)
    {
        transitionMaterial.SetFloat(_size, 0);
        StartCoroutine(ExecuteTransition(1, onComplete, delay));
    }

    public void ToBlack(float delay = 0f, Action onComplete = null)
    {
        transitionMaterial.SetFloat(_size, 1);
        StartCoroutine(ExecuteTransition(0, onComplete, delay));
    }

    private IEnumerator ExecuteTransition(float targetValue, Action onComplete, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        transitionMaterial.DOFloat(targetValue, _size, 0.7f).SetEase(Ease.InOutSine).OnComplete(() => onComplete?.Invoke());
    }
}