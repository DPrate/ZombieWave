using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Weapon[] Weapons;
    public Weapon StartingWeapon;
    public Weapon DefaultWeapon;
    private Weapon currentWeapon;

    public string MecanimShoot = "Shoot";

    public LayerMask Hittables;
    private Transform cachedTransform;
    private Animator cachedAnimator;

    private IEnumerator EquipDurationCoroutine = null;

    private void Start()
    {
        CacheObjects();

        if(StartingWeapon != null)
            EquipWeapon(StartingWeapon);

        for(int i = 0; i < Weapons.Length; i++)
        {
            if(Weapons[i] != currentWeapon)
                Weapons[i].WeaponMesh.enabled = false;
        }
    }

    private void Update()
    {
        GetTouch();
    }

    private void CacheObjects()
    {
        if(cachedTransform == null)
            cachedTransform = transform;

        if(cachedAnimator == null)
            cachedAnimator = gameObject.GetComponent<Animator>();
    }

    private void GetTouch()
    {
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100.0f, Hittables))
            {
                if(hit.transform.CompareTag("Enemy"))
                {
                    if(currentWeapon.pIsReadyToFire)
                    {
                        cachedTransform.LookAt(hit.transform);
                        cachedAnimator.SetTrigger(MecanimShoot);

                        Enemy hitEnemy = hit.transform.GetComponent<Enemy>();
                        currentWeapon.FireWeapon(hitEnemy);
                    }
                }
                else if(hit.transform.CompareTag("Collectable"))
                {
                    Collectable collectable = hit.transform.GetComponent<Collectable>();
                    EquipWeapon(GetCorrectWeapon(collectable.WeaponType));
                    collectable.Despawn();
                }
            }
        }
    }

    private void UnequipCurrentWeapon()
    {
        if(currentWeapon != null)
        {
            currentWeapon.WeaponMesh.enabled = false;
            cachedAnimator.SetLayerWeight(currentWeapon.MecanimLayer, 0.0f);
        }
    }

    private void EquipWeapon(Weapon weaponToEquip)
    {
        UnequipCurrentWeapon();

        weaponToEquip.WeaponMesh.enabled = true;
        cachedAnimator.SetLayerWeight(weaponToEquip.MecanimLayer, 1.0f);
        currentWeapon = weaponToEquip;

        if(currentWeapon != DefaultWeapon)
        {
            if(EquipDurationCoroutine != null)
                StopCoroutine(EquipDurationCoroutine);

            EquipDurationCoroutine = RunEquipDuration(currentWeapon.EquipDuration);
            StartCoroutine(EquipDurationCoroutine);
        }
    }

    private Weapon GetCorrectWeapon(WeaponType inWeaponType)
    {
        for(int i = 0; i < Weapons.Length; i++)
        {
            if(Weapons[i].WeaponType == inWeaponType)
                return Weapons[i];
        }

        return null;
    }

    private IEnumerator RunEquipDuration(float duration)
    {
        float timer = duration;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        EquipDurationCoroutine = null;
        EquipWeapon(DefaultWeapon);
    }
}

