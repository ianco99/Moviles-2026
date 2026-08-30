using System;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private Quaternion rot;
    private void Start()
    {
        rot = transform.localRotation;
    }

    void Update()
    {
        transform.localRotation = rot;
    }
}
