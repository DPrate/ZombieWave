using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using Diggames.Utilities;

public class Enemy : MonoBehaviour
{
    public string EnemyPoolName = "Enemy";

    public string BloodSplatPoolName = "BloodSplat";
    public Transform[] BloodSplatPositions;

    public float Health = 100.0f;
    private float currentHealth;

    public float MoveSpeed = 1.0f;

    public string MecanimWalk = "Walk";
    public string MecanimDeath = "Death";
    public string MecanimTakeHit = "TakeHit";
    public float TakeHitStopDuration = 1.0f;

    public float DeathCorpseDuration = 1.5f;

    private Transform cachedTransform;
    private splineMove splineMovement;
    private Animator animator;
    private Collider hitCollider;

    private IEnumerator TakeDamageDelayCoroutine = null;

    public void Initialize(PathManager inPath)
    {
        CacheObjects();
        SetMovement(inPath);
        SetHealth();
    }

    private void CacheObjects()
    {
        if(cachedTransform == null)
            cachedTransform = transform;

        if(animator == null)
            animator = gameObject.GetComponent<Animator>();

        if(splineMovement == null)
            splineMovement = gameObject.GetComponent<splineMove>();

        if(hitCollider == null)
            hitCollider = gameObject.GetComponent<Collider>();
    }

    private void SetMovement(PathManager inPath)
    {
        if(splineMovement != null)
        {
            splineMovement.SetPath(inPath);
            splineMovement.speed = MoveSpeed;
        }  
        else
            DebugLogger.LogMessage("No splineMove on: " + gameObject.name, DebugLogger.LogType.WARNING);
    }

    private void SetHealth()
    {
        if(hitCollider != null)
            hitCollider.enabled = true;
        else
            DebugLogger.LogMessage("No hit collider on: " + gameObject.name, DebugLogger.LogType.WARNING);

        currentHealth = Health;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if(currentHealth > 0)
        {
            splineMovement.Pause(3.0f);
            animator.SetBool(MecanimWalk, false);
            animator.SetTrigger(MecanimTakeHit);

            if(TakeDamageDelayCoroutine != null)
                StopCoroutine(TakeDamageDelayCoroutine);

            TakeDamageDelayCoroutine = TakeDamageDelay();
            StartCoroutine(TakeDamageDelayCoroutine);

        }
        else
            Die();

        if(!string.IsNullOrEmpty(BloodSplatPoolName))
        {
            if(BloodSplatPositions.Length > 0)
                PoolManager.singletonInstance.InstantiatePoolObject(BloodSplatPoolName, BloodSplatPositions[Random.Range(0, BloodSplatPositions.Length)].position, Quaternion.identity);
            else
                PoolManager.singletonInstance.InstantiatePoolObject(BloodSplatPoolName, transform.position, Quaternion.identity);
        }
            
            
    }

    private IEnumerator TakeDamageDelay()
    {
        yield return new WaitForSeconds(TakeHitStopDuration);
        splineMovement.Resume();
        animator.SetBool(MecanimWalk, true);
        TakeDamageDelayCoroutine = null;
    }

    public void Die()
    {
        if(TakeDamageDelayCoroutine != null)
            StopCoroutine(TakeDamageDelayCoroutine);

        hitCollider.enabled = false;
        splineMovement.Stop();
        animator.SetBool(MecanimDeath, true);
        StartCoroutine(CorpseDelay());
    }

    private void Deinitialize()
    {
        animator.SetBool(MecanimDeath, false);
        CollectableManager.Instance.CheckForRandomDrop(cachedTransform.position);
        PoolManager.singletonInstance.DespawnPoolObject(EnemyPoolName, gameObject);
        EnemyManager.singletonInstance.RemoveEnemyFromCurrent(this);
        
    }

    private IEnumerator CorpseDelay()
    {
        float timer = DeathCorpseDuration;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        Deinitialize();
    }
}
