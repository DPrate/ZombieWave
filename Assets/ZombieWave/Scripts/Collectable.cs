using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public WeaponType WeaponType;
    public string PoolName = "CollectablePistol";

    public void Despawn()
    {
        PoolManager.singletonInstance.DespawnPoolObject(PoolName, gameObject);
    }
}
