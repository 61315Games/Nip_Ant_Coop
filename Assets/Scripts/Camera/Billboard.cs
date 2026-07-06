using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;
    void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(cam.forward, Vector3.up);
    }
}
