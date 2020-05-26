using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
    public const float aggressionRadius = 5f;
    private const float checkPeriod = 1f/10f;

    public static bool aggressive = false;

    private GameObject[] agents;
    private GameObject player;
    private float aggressionRadiusSquared = aggressionRadius*aggressionRadius;

    private void GetAgents()
    {
        agents = GameObject.FindGameObjectsWithTag("agent");
    }

    private void Start()
    {
        GetAgents();
        player = GameObject.Find("player");
    }

    private void EvaluateAggression()
    {
        for (uint i = 0; i < agents.Length; ++i)
        {
            Vector3 toPlayer = player.transform.position - agents[i].transform.position;
            if (toPlayer.sqrMagnitude <= aggressionRadiusSquared)
                aggressive = true;
        }
    }

    float timer = 0f;
    private void Update()
    {
        if (aggressive)
            return;

        if (timer >= checkPeriod)
        {
            EvaluateAggression();
            timer -= checkPeriod;
        }

        timer += Time.deltaTime;
    }
}