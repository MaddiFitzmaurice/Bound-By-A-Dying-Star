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

    // Fades a scene to black over the set time
    public IEnumerator NormalFadeOut()
    {
        LeanTween.alphaCanvas(_fadeOutBG, 1f, _fadeOutTime).setFrom(0f);

        while (LeanTween.isTweening(_fadeOutBG.gameObject))
        {
            yield return null;
        }
    }

    // Fades from black to the scene over the set time
    public IEnumerator NormalFadeIn()
    {
        LeanTween.alphaCanvas(_fadeOutBG, 0f, _fadeOutTime).setFrom(1f);

        while (LeanTween.isTweening(_fadeOutBG.gameObject))
        {
            yield return null;
        }
    }
}
