using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAtPlayer : MonoBehaviour
{
    private Transform player;
    private Vector3 toPlayer = Vector3.forward;
    private Vector3 toPlayerAbsolute = Vector3.forward;

    private void Start()
    {
        player = GameObject.Find("player").transform;
    }

    private void LateUpdate()
    {
        Vector3 toPlayerAbsolute = player.position - transform.position;
        toPlayerAbsolute.y = 0;

        toPlayer = Vector3.Lerp(toPlayer, toPlayerAbsolute, 0.4f);

        transform.rotation = Quaternion.LookRotation(toPlayer, Vector3.up);
    }
}
