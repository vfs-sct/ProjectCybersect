using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;

public class GrappleRopeVisuals : MonoBehaviour
{
    public float sinDuration = 0.75f;
    public float sinSize = 3f;
    public float sinFrequency = 10f;
    public float sinSpeed = 5f;

    private Mesh mesh;
    private Grapple grapple;
    private FPSLook look;

    private Vector3[] initialVertices;

    private Vector3[] vertexSwapA;
    private Vector3[] vertexSwapB;
    private Vector3[] vertexSwap;

    private int vertexCount;
    private float timer = 0f;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        vertexCount = mesh.vertexCount;

        initialVertices = mesh.vertices;

        vertexSwapA = new Vector3[vertexCount];
        vertexSwapB = new Vector3[vertexCount];

        vertexSwap = vertexSwapA;

        GameObject player = GameObject.Find("player");
        grapple = player.GetComponent<Grapple>();
        look = player.GetComponent<FPSLook>();
    }

    private float WobbleFunction(Vector3 vert, float frequency)
    {
        return Mathf.Sin(Time.time * sinSpeed + (vert.z / transform.localScale.z) * sinFrequency * frequency) * sinSize;
    }

    private void Wobble()
    {
        if (timer > sinDuration + 0.5f)
            return;

        for (int i = 0; i < vertexCount; ++i)
        {
            float value = sinDuration - timer;
            if (value < 0f)
                value = 0f;
            vertexSwap[i] = initialVertices[i] + Vector3.right * (WobbleFunction(initialVertices[i], 0.4f)*2f + WobbleFunction(initialVertices[i], 1))*value;
        }

        if (vertexSwap == vertexSwapA)
        {
            vertexSwap = vertexSwapB;
            mesh.vertices = vertexSwapA;
        }
        else
        {
            vertexSwap = vertexSwapA;
            mesh.vertices = vertexSwapB;
        }

        timer += Time.deltaTime;
    }

    private void Bend()
    {

    }

    private void Update()
    {
        Wobble();
        Bend();
    }
}
