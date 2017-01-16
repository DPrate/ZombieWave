using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Diggames.Utilities;

public class PoolManager : MonoBehaviour
{
    public List<PoolObject> pools = new List<PoolObject>();
    public static PoolManager singletonInstance = null;

	void Awake()
    {
        if(singletonInstance == null)
            singletonInstance = this;
        else
            Destroy(gameObject);
	}

    void Start()
    {
        if(pools.Count > 0)
        {
            foreach(PoolObject poolObject in pools)
            {
                for(int i = 0; i < poolObject.PrePoolCount; i++)
                {
                    poolObject.InstantiateObjectForPool(transform.position, transform);
                }
            }
        }
    }

    public GameObject InstantiatePoolObject(string name, Vector3 spawnPoint, Quaternion spawnRotation)
    {
        PoolObject poolObject = GetPool(name);
        GameObject poolObjectSpawned = null;

        if(poolObject != null)
        {
            poolObjectSpawned = poolObject.SpawnObject(spawnPoint, spawnRotation, transform);

            DestroyAfterSeconds destroyAfterSeconds = poolObjectSpawned.GetComponent<DestroyAfterSeconds>();

            if(destroyAfterSeconds != null)
                destroyAfterSeconds.StartDestroy(poolObjectSpawned, name);
        }
        else
        {
            DebugLogger.LogMessage("Trying to spawn from a pool with no Pools setup!!!", DebugLogger.LogType.ERROR);
        }

        return poolObjectSpawned;
    }

    public void DespawnPoolObject(string name, GameObject poolObjectToDespawn)
    {
        PoolObject poolObject = GetPool(name);
        poolObject.DespawnObject(poolObjectToDespawn);
    }

    private PoolObject GetPool(string name)
    {
        for(int i = 0; i < pools.Count; i++)
        {
            if(pools[i].name == name)
            {
                return pools[i];
            }
        }

        DebugLogger.LogMessage("No pool found with name " + name, DebugLogger.LogType.WARNING);

        return null;
    }

    public void DespawnEntirePool(string poolToDespawn)
    {
        PoolObject poolObject = GetPool(poolToDespawn);
        poolObject.DespawnEntirePool();
    }
}

[Serializable]
public class PoolObject
{
    public string name;
    private List<GameObject> inactivePooledGameObjects = new List<GameObject>();
    private List<GameObject> activePooledGameObjects = new List<GameObject>();
    public GameObject[] objectToPool;
    public int PrePoolCount;

    public GameObject SpawnObject(Vector3 spawnPoint, Quaternion spawnRotation, Transform parentObject)
    {
        GameObject poolObjectSpawned = null;

        if(inactivePooledGameObjects.Count == 0)
        {
            DebugLogger.LogMessage("No pool object available from inactive pool, instantiating new object.", DebugLogger.LogType.WARNING);

            InstantiateObjectForPool(spawnPoint, parentObject);
        }

        DebugLogger.LogMessage("Spawning object from inactive pool.");
        poolObjectSpawned = inactivePooledGameObjects[0];
        poolObjectSpawned.transform.position = spawnPoint;
        poolObjectSpawned.transform.rotation = spawnRotation;
        inactivePooledGameObjects.Remove(poolObjectSpawned);
        activePooledGameObjects.Add(poolObjectSpawned);
        poolObjectSpawned.SetActive(true);
        return poolObjectSpawned;
    }

    public void DespawnObject(GameObject spawnedObject)
    {
        if(!activePooledGameObjects.Contains(spawnedObject))
        {
            DebugLogger.LogMessage("Attempting to despawn an object that is already despawned!", DebugLogger.LogType.WARNING);
            return;
        }

        activePooledGameObjects.Remove(spawnedObject);
        inactivePooledGameObjects.Add(spawnedObject);
        spawnedObject.SetActive(false);
    }

    public void DespawnEntirePool()
    {
        while(activePooledGameObjects.Count > 0)
            DespawnObject(activePooledGameObjects[0]);
    }

    public void InstantiateObjectForPool(Vector3 spawnPoint, Transform parentObject)
    {
        GameObject pooledObject = null;

        if(objectToPool.Length > 0)
        {
            pooledObject = GameObject.Instantiate(objectToPool[UnityEngine.Random.Range(0, objectToPool.Length)], spawnPoint, Quaternion.identity) as GameObject;
        }
        else
        {
            pooledObject = GameObject.Instantiate(objectToPool[0], spawnPoint, Quaternion.identity) as GameObject;
        }

        inactivePooledGameObjects.Add(pooledObject);

        //If in the editor child for easier viewing, else do not to optimize movement on non-child objects.
#if UNITY_EDITOR
        pooledObject.transform.parent = parentObject;
#endif
        pooledObject.SetActive(false);
    }
}
