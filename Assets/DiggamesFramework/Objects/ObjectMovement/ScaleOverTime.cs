using UnityEngine;
using System.Collections;

public class ScaleOverTime : MonoBehaviour
{
    public Vector3 startScale;
    public Vector3 endScale;
    public float scaleTime = 1.0f;

    public bool useCurrentScale = false;
    public bool useUnscaledTime = false;
    public bool deactivateAfterScale = true;

    void OnEnable()
    {
        StartScale();
    }

    public void StartScale()
    {
        if(useCurrentScale)
            startScale = transform.localScale;

        StartCoroutine(RunScale());
    }

    private IEnumerator RunScale()
    {
        float currentTime = 0.0f;
        float lerpPercentage = 0.0f;

        while(lerpPercentage < 1.0f)
        {
            lerpPercentage = currentTime / scaleTime;

            transform.localScale = Vector3.Lerp(startScale, endScale, lerpPercentage);

            if(!useUnscaledTime)
                currentTime += Time.deltaTime;
            else
                currentTime += Time.unscaledDeltaTime;

            yield return null;
        }

        if(deactivateAfterScale)
            gameObject.SetActive(false);
    }
}
