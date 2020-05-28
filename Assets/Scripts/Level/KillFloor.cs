// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFloor : MonoBehaviour
{
    [SerializeField] private PlayerStats _player = null;

    private void OnTriggerEnter(Collider other)
    {
        _player.TakeDamage(999999);
    }
}
