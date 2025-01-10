using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _targetPlayer;
    [SerializeField] private Vector3 offset;

    void Update()
    {
        transform.position = _targetPlayer.transform.position + offset;
    }
}
