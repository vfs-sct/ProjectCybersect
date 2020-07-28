using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public const float aggressionRadius = 5f;
    public const float decisionTickRate = 0.5f;
    public const float aggressionCheckRate = 1f;

    public static bool aggressive = false;
    public BehaviourTree behaviourTree = null;
    
    private GameObject player;
    private float aggressionCheckTimer = 0f;
    private float decisionTimer = 0f;

    private float aggressionRadiusSquared = aggressionRadius*aggressionRadius;

    public void Start()
    {
        player = GameObject.Find("player");
    }
    
    private void EvaluateAggression()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        if (toPlayer.sqrMagnitude <= aggressionRadiusSquared)
            aggressive = true;
        else
            aggressive = false;
    }

    private void UpdateAggression()
    {
        if (aggressionCheckTimer >= aggressionCheckRate)
        {
            EvaluateAggression();
            aggressionCheckTimer -= aggressionCheckRate;
        }

        aggressionCheckTimer += Time.deltaTime;
    }

    private void UpdateDecision()
    {
        if (behaviourTree == null)
            return;

        if (decisionTimer >= decisionTickRate)
        {
            behaviourTree.Execute();
            decisionTimer -= decisionTickRate;
        }

        decisionTimer += Time.deltaTime;
    }

    public void Update()
    {
        UpdateAggression();
        UpdateDecision();
    }
}
