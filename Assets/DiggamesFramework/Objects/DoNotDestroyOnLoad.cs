using UnityEngine;
using System.Collections;

public class DoNotDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
