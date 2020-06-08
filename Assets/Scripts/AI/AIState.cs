using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
    private const float checkPeriod = 1f/10f;

    public static bool aggressive = true;
    public static float density = 0f;
    public static Vector3 averagePosition;
    public static int agentCount = 0;

    private GameObject[] agents;
    private GameObject player;
    private float aggressionRadiusSquared = AI.aggressionRadius*AI.aggressionRadius;

    private void GetAgents()
    {
        agents = GameObject.FindGameObjectsWithTag("agent");
        agentCount = agents.Length;
    }

    private void Start()
    {
        GetAgents();
        player = GameObject.Find("player");
    }

    private void EvaluateAggression()
    {
        if (aggressive)
            return;

        for (uint i = 0; i < agents.Length; ++i)
        {
            if (agents[i] == null)
                continue;

            Vector3 toPlayer = player.transform.position - agents[i].transform.position;
            if (toPlayer.sqrMagnitude <= aggressionRadiusSquared)
                aggressive = true;
        }
    }
    
    float timer = 0f;
    private void Update()
    {
        if (timer >= checkPeriod)
        {
            EvaluateAggression();
            timer -= checkPeriod;
        }

        timer += Time.deltaTime;
    }
}