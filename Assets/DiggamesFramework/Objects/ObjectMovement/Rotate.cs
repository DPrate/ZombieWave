using UnityEngine;
using System.Collections;

namespace Diggames
{
    public class Rotate : MonoBehaviour
    {
        public Vector3 rotationSpeed;

        void Update()
        {
            transform.Rotate(rotationSpeed.x * Time.deltaTime, rotationSpeed.y * Time.deltaTime, rotationSpeed.z * Time.deltaTime);
        }
    }
}
