using UnityEngine;
using System.Collections;

public class MoveDirection : MonoBehaviour
{
    public float moveSpeed;
    public Vector3 movementVector;

	void Update ()
    {
        transform.localPosition += movementVector * Time.deltaTime * moveSpeed;
	}

    public void PopUp(float duration)
    {
        StartCoroutine(RunPopUp(duration));
    }

    private IEnumerator RunPopUp(float duration)
    {
        float timer = duration;
        Vector3 originalDirection = movementVector;
        movementVector = Vector3.up;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        movementVector = originalDirection;
    }
}
