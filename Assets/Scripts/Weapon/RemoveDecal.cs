// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveDecal : MonoBehaviour
{
    private void OnCollisionExit(Collision other)
    {
        Destroy(gameObject);
    }
}
