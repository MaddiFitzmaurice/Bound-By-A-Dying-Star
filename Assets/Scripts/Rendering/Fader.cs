using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] GameObject _fadeBG;
    [SerializeField] float _fadeInTime;
    [SerializeField] float _fadeOutTime;
    private CanvasGroup _fadeOutBG;
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _fadeOutBG = GetComponentInChildren<CanvasGroup>();
    }

    public IEnumerator NormalFadeOut()
    {
        LeanTween.alphaCanvas(_fadeOutBG, 1f, _fadeOutTime).setFrom(0f);

        while (LeanTween.isTweening(_fadeOutBG.gameObject))
        {
            yield return null;
        }
    }

    public IEnumerator NormalFadeIn()
    {
        LeanTween.alphaCanvas(_fadeOutBG, 0f, _fadeOutTime).setFrom(1f);

        while (LeanTween.isTweening(_fadeOutBG.gameObject))
        {
            yield return null;
        }
    }

    private Vector3 WorldToUI(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        Vector2 circlePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, _canvas.worldCamera, out circlePos);

        return _canvas.transform.TransformPoint(circlePos);
    }
}
