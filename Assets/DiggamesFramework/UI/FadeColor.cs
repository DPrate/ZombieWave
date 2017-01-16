using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class FadeColor : MonoBehaviour
{
    public event Action OnFadeComplete;           //This is called when the fade in or out has finished.

    public MaskableGraphic FadeGraphic;

    public Color StartColor = Color.white;
    public Color FadeInColor;
    public Color FadeOutColor;

    public float FadeInDuration = 2.0f;
    public float FadeOutDuration = 1.0f;
    public float FadeInitialDelay = 1.0f;

    public bool DoesFadeInOnEnable = false;
    public bool DoesFadeOutOnEnable = false;

    private IEnumerator fadeCoroutine = null;
    private Color currentFadeColor;

    private bool isFading = false;
    public bool pIsFading
    {
        get
        {
            return isFading;
        }
    }

    private void Start()
    {
        if(FadeGraphic == null)
        {
            Debug.LogError("FadeColor component on object " + gameObject.name + " has no Fade Graphic assigned!");
            return;
        }

        FadeGraphic.color = StartColor;
        currentFadeColor = FadeGraphic.color;
    }

    private void OnEnable()
    {
        if(DoesFadeInOnEnable)
        {
            FadeIn();
        }
        else if(DoesFadeOutOnEnable)
        {
            FadeOut();
        }
    }

    public void FadeOut()
    {
        if(fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = RunFade(currentFadeColor, FadeOutColor, FadeOutDuration, false);

        StartCoroutine(fadeCoroutine);
    }

    public void FadeIn()
    {
        if(fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = RunFade(currentFadeColor, FadeInColor, FadeInDuration, true);
        StartCoroutine(fadeCoroutine);
    }

    private IEnumerator RunFade(Color startColor, Color endColor, float duration, bool isFadeIn)
    {
        float timer = 0.0f;
        isFading = true;

        if(isFadeIn && FadeInitialDelay > 0)
        {
            while(timer < FadeInitialDelay)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        timer = 0.0f;

        if(FadeGraphic != null)
        {
            while(timer <= duration)
            {
                FadeGraphic.color = Color.Lerp(startColor, endColor, timer / duration);
                currentFadeColor = FadeGraphic.color;

                timer += Time.deltaTime;
                yield return null;
            }

            FadeGraphic.color = endColor;

            currentFadeColor = FadeGraphic.color;
            isFading = false;

            // If anything is subscribed to OnFadeComplete call it.
            if(OnFadeComplete != null)
                OnFadeComplete();
        }
        else
            Debug.LogError("FadeColor component on object " + gameObject.name + " has no Fade Graphic assigned!");

        fadeCoroutine = null;
    }
}
