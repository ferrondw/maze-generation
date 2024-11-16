using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType
    {
        Self,
        Side
    }

    public ButtonType buttonType;

    private const float scaleMultiplier = 0.9f;
    private const float animTime = 0.25f;
    private const float selfOffset = 30f;
    private const Ease easeIn = Ease.OutCubic;
    private const Ease easeOut = Ease.OutCubic;
    
    private Vector3 originalScale;
    private Vector3 originalPosition;

    private void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    public void OnPointerDown(PointerEventData eventData) => EnterButton();
    public void OnPointerUp(PointerEventData eventData) => ExitButton();

    private void ExitButton()
    {
        switch (buttonType)
        {
            case ButtonType.Self:
                transform.DOScale(originalScale, animTime).SetEase(easeOut);
                break;
            case ButtonType.Side:
                transform.DOLocalMoveX(originalPosition.x, animTime).SetEase(easeOut);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void EnterButton()
    {
        switch (buttonType)
        {
            case ButtonType.Self:
                transform.DOScale(originalScale * scaleMultiplier, animTime).SetEase(easeIn);
                break;
            case ButtonType.Side:
                Vector3 targetLocalPosition = transform.localPosition + transform.TransformDirection(Vector3.left * selfOffset);
                transform.DOLocalMoveX(targetLocalPosition.x, animTime).SetEase(easeIn);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}