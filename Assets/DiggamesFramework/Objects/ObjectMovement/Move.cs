using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    public float moveSpeed;
    public Transform target = null;

    void Update()
    {
        if(target == null)
        {
            transform.localPosition += transform.forward * Time.deltaTime * moveSpeed;
        }
        else
        {
            MoveTowards(target.position);
        }
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, target.position) <= 0.5f)
            target = null;
    }
}
