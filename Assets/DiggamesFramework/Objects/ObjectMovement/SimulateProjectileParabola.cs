using UnityEngine;
using System.Collections;

public class SimulateProjectileParabola : MonoBehaviour
{
    public Transform projectile;
    public Transform shootingPoint;

    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    public string ExplosionFXPoolName;
    public string groundFXPoolName;

    private Transform myTransform;
    private GameObject myGameobject;
    private Vector3 targetPosition;

    void Awake()
    {
        myGameobject = gameObject;
        myTransform = transform;
        myGameobject.SetActive(false);
    }

    public void Launch(Vector3 position)
    {
        myGameobject.SetActive(true);
        myTransform.position = shootingPoint.position;
        targetPosition = position;
        StartCoroutine(SimulateProjectile());
    }

    IEnumerator SimulateProjectile()
    {
        // Move projectile to the position of throwing object + add some offset if needed.
        projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(projectile.position, targetPosition);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        projectile.rotation = Quaternion.LookRotation(targetPosition - projectile.position);

        float elapse_time = 0;

        while(elapse_time < flightDuration)
        {
            projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        if(!string.IsNullOrEmpty(ExplosionFXPoolName))
            PlayFX(ExplosionFXPoolName);

        if(!string.IsNullOrEmpty(groundFXPoolName))
            PlayFX(groundFXPoolName);

        myGameobject.SetActive(false);
    }

    private void PlayFX(string fx)
    {
        PoolManager.singletonInstance.InstantiatePoolObject(fx, targetPosition, Quaternion.identity);     
    }
}

