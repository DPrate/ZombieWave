using UnityEngine;
using System;
using System.Collections;

public class Utilities : MonoBehaviour
{
    public static IEnumerator WaitForUnscaledSeconds(float time)
    {
        float ttl = 0;
        while(time > ttl)
        {
            ttl += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    public static bool LayerMaskContainsLayer(LayerMask layerMask, int layer)
    {
        if(layerMask == (layerMask | (1 << layer)))
            return true;

        return false;
    }
}

[Serializable]
public class MinMax
{
    public float Min;
    public float Max;

    public float GetRandomBetween()
    {
        return UnityEngine.Random.Range(Min, Max);
    }
}
