using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using Diggames.Utilities;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager singletonInstance = null;

    public string EnemyPool = "Enemy";
    public int StartingEnemies = 3;
    public int MaximumEnemies = 20;
    public float EnemySpawnTimer = 3.0f;
    public bool BalanceEnemiesOnPaths = true;
    public List<EnemyPathInfo> EnemyPaths = new List<EnemyPathInfo>();

    private List<Enemy> currentEnemies = new List<Enemy>();

    //This list holds enemy paths that have the least amount of enemies on to ensure even spawning of enemies on the numerous paths.
    private List<EnemyPathInfo> SpawnableEnemyPaths = new List<EnemyPathInfo>();

    void Awake()
    {
        if(singletonInstance == null)
            singletonInstance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        while(currentEnemies.Count < StartingEnemies)
            SpawnEnemy();

        StartCoroutine(RunEnemySpawner());
    }

    private void SpawnEnemy()
    {
        if(BalanceEnemiesOnPaths)
        {
            SpawnableEnemyPaths.Clear();
            EnemyPaths.Sort((a, b) => a.pEnemiesOnPath.CompareTo(b.pEnemiesOnPath));

            int lowestEnemyCountOnPath = EnemyPaths[0].pEnemiesOnPath;

            for(int i = 0; i < EnemyPaths.Count; i++)
            {
                if(EnemyPaths[i].pEnemiesOnPath <= lowestEnemyCountOnPath)
                    SpawnableEnemyPaths.Add(EnemyPaths[i]);
                else
                    break;
            }

            int randomSpawn = UnityEngine.Random.Range(0, SpawnableEnemyPaths.Count);
            Enemy newEnemy = PoolManager.singletonInstance.InstantiatePoolObject(EnemyPool, SpawnableEnemyPaths[randomSpawn].EnemyPath.waypoints[0].position, SpawnableEnemyPaths[randomSpawn].EnemyPath.waypoints[0].rotation).GetComponent<Enemy>();
            currentEnemies.Add(newEnemy);

            EnemyPathInfo pathInfo = EnemyPaths.Find(x => x == SpawnableEnemyPaths[randomSpawn]);
            pathInfo.pEnemiesOnPath++;

            newEnemy.Initialize(pathInfo.EnemyPath);
        }
        else
        {
            int randomSpawn = UnityEngine.Random.Range(0, EnemyPaths.Count);
            Enemy newEnemy = PoolManager.singletonInstance.InstantiatePoolObject(EnemyPool, EnemyPaths[randomSpawn].EnemyPath.waypoints[0].position, EnemyPaths[randomSpawn].EnemyPath.waypoints[0].rotation).GetComponent<Enemy>();
            currentEnemies.Add(newEnemy);

            EnemyPaths[randomSpawn].pEnemiesOnPath++;
            newEnemy.Initialize(EnemyPaths[randomSpawn].EnemyPath);
        }
    }

    private IEnumerator RunEnemySpawner()
    {
        float timer = EnemySpawnTimer;

        while(timer > 0.0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if(currentEnemies.Count < MaximumEnemies)
            SpawnEnemy();

        StartCoroutine(RunEnemySpawner());
    }

    public void RemoveEnemyFromCurrent(Enemy inEnemy)
    {
        currentEnemies.Remove(inEnemy);
    }
}

[Serializable]
public class EnemyPathInfo
{
    public PathManager EnemyPath;

    private int enemiesOnPath = 0;
    public int pEnemiesOnPath
    {
        get
        {
            return enemiesOnPath;
        }
        set
        {
            enemiesOnPath = value;
        }
    }
}
