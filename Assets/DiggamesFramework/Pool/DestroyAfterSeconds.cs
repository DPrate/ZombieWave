using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class DestroyAfterSeconds : MonoBehaviour
{
    public float secondsToWait;
    public string poolName;

    public void StartDestroy(GameObject objectToDestroy, string poolName = null)
    {
        StartCoroutine(DestroyAfterTime(objectToDestroy, poolName));    
    }

    private IEnumerator DestroyAfterTime(GameObject objectToDestroy, string poolName = null)
    {
        float timer = secondsToWait;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if(!string.IsNullOrEmpty(poolName))
            PoolManager.singletonInstance.DespawnPoolObject(poolName, objectToDestroy);
        else
            Destroy(objectToDestroy);
    }
}
