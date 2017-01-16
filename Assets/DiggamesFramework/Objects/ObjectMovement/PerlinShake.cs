using UnityEngine;
using System.Collections;

public class PerlinShake : MonoBehaviour
{
    public float Duration = 2f;
    public float Speed = 20f;
    public float Magnitude = 2f;
    public AnimationCurve Damper = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.9f, .33f, -2f, -2f), new Keyframe(1f, 0f, -5.65f, -5.65f));

    IEnumerator Shake(Transform transform, Vector3 originalPosition, float duration, float speed, float magnitude, AnimationCurve damper = null)
    {
        float elapsed = 0f;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float damperedMag = (damper != null) ? (damper.Evaluate(elapsed / duration) * magnitude) : magnitude;
            float x = (Mathf.PerlinNoise(Time.time * speed, 0f) * damperedMag) - (damperedMag / 2f);
            float y = (Mathf.PerlinNoise(0f, Time.time * speed) * damperedMag) - (damperedMag / 2f);
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            yield return null;
        }

        transform.localPosition = originalPosition;
    }


    IEnumerator ShakeCamera(Camera camera, float duration, float speed, float magnitude, AnimationCurve damper = null)
    {
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float damperedMagnitude = (damper != null) ? (damper.Evaluate(elapsedTime / duration) * magnitude) : magnitude;
            float x = (Mathf.PerlinNoise(Time.time * speed, 0f) * damperedMagnitude) - (damperedMagnitude / 2f);
            float y = (Mathf.PerlinNoise(0f, Time.time * speed) * damperedMagnitude) - (damperedMagnitude / 2f);

            float frustrumHeight = 2 * camera.nearClipPlane * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustrumWidth = frustrumHeight * camera.aspect;
            Matrix4x4 mat = camera.projectionMatrix;
            mat[0, 2] = 2 * x / frustrumWidth;
            mat[1, 2] = 2 * y / frustrumHeight;
            camera.projectionMatrix = mat;
            yield return null;
        }

        camera.ResetProjectionMatrix();
    }
}
