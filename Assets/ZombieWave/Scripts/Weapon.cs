using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    PISTOL,
    SMG,
    SHOTGUN,
    ASSAULTRIFLE,
    MACHINEGUN,
    SNIPERRIFLE,
}

public class Weapon : MonoBehaviour
{
    public WeaponType WeaponType;
    public Renderer WeaponMesh;
    public int MecanimLayer;

    public string MuzzleFlashPool = "MuzzleFlash";

    public float Damage = 10.0f;
    public float CooldownDuration = 1.0f;

    public Transform ShootingPoint;

    private bool isReadyToFire = true;
    public bool pIsReadyToFire
    {
        get
        {
            return isReadyToFire;
        }
    }

    public void FireWeapon(Enemy enemy)
    {
        if(!isReadyToFire)
            return;

        PlayMuzzleFlash();
        DoDamage(enemy);
        StartCoroutine(RunCooldown());
    }

    private void PlayMuzzleFlash()
    {
        if(!string.IsNullOrEmpty(MuzzleFlashPool))
        {
            if(ShootingPoint != null)
                PoolManager.singletonInstance.InstantiatePoolObject(MuzzleFlashPool, ShootingPoint.position, Quaternion.identity);
        }

    }

    private void DoDamage(Enemy enemy)
    {
        enemy.TakeDamage(Damage);
    }

    private IEnumerator RunCooldown()
    {
        isReadyToFire = false;
        float timer = CooldownDuration;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isReadyToFire = true;
    }
}
