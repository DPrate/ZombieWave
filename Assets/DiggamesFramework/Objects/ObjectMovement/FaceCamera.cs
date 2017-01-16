using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour
{
    public Camera sourceCamera;

    void OnEnable()
    {
        if(sourceCamera == null)
            sourceCamera = Camera.main;
    }

    void Update()
    {
        if(sourceCamera != null)
        {
            transform.rotation = sourceCamera.transform.rotation;
        }
    }
}
