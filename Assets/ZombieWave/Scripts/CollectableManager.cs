using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager Instance;

    public List<CollectableInfo> Collectables = new List<CollectableInfo>();

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Collectables.Sort((a, b) => a.DropPercentage.CompareTo(b.DropPercentage));
    }

    public void CheckForRandomDrop(Vector3 dropPosition)
    {
        for(int i = 0; i < Collectables.Count; i++)
        {
            int random = UnityEngine.Random.Range(0, 100);

            if(random < Collectables[i].DropPercentage)
            {
                if(!string.IsNullOrEmpty(Collectables[i].PoolName))
                    PoolManager.singletonInstance.InstantiatePoolObject(Collectables[i].PoolName, dropPosition + Collectables[i].DropOffset, Quaternion.Euler(Collectables[i].DropRotation));
                else
                    Debug.LogError("No PoolName found for: " + Collectables[i].Weapon.ToString());

                break;
                      
            }
        }
    }
}

[Serializable]
public class CollectableInfo
{
    public WeaponType Weapon;
    public int DropPercentage;
    public string PoolName;

    public Vector3 DropOffset;
    public Vector3 DropRotation;
}
