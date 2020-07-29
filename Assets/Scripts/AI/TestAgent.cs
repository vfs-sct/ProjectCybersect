using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestAgent : AI
{
    public bool accountForBulletDrop = true;
    public float fireRate = 0.75f;
    public float closeProximity = 5f;
    public float farProximity = aggressionRadius - 5f;
    public GameObject projectilePrefab;
    public Transform projectileReleaseTransform = null;

    private NavMeshAgent navMeshAgent;
    private PlayerStats playerStats;
    private Transform playerTransform;
    private Transform mainCamera;
    private EnemyStats enemyStats;

    private bool pathedToPlayer = false;
    private bool los = false;
    private Vector3 toPlayer = Vector3.zero;
    private float toPlayerSqrMag = 0f;
    private float distanceToPlayer = 0f;
    private Random random = new Random();
    private int xStrafeDir = 0;
    private int zStrafeDir = 0;

    private float fireTimer = 0f;
    private float decisionTimer = 0f;

    private void EvaluateToPlayer()
    {
        toPlayer = playerTransform.position - transform.position;
        toPlayerSqrMag = toPlayer.sqrMagnitude;
        distanceToPlayer = toPlayer.magnitude;
    }

    private void CheckLOS()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, toPlayer, out hit))
        {
            if (hit.transform == playerTransform.transform)
                los = true;
            else
                los = false;

            return;
        }

        los = false;
    }

    private void PathToPlayer()
    {
        navMeshAgent.SetDestination(playerTransform.position);
        pathedToPlayer = true;
    }

    private void ClearPath()
    {
        navMeshAgent.ResetPath();
        pathedToPlayer = false;
    }

    public override void Start()
    {
        base.Start();

        navMeshAgent = GetComponent<NavMeshAgent>();
        playerStats = GameObject.Find("player").GetComponent<PlayerStats>();
        playerTransform = GameObject.Find("player").transform;
        mainCamera = GameObject.Find("mainCamera").transform;
        enemyStats = GetComponent<EnemyStats>();

        if (projectileReleaseTransform == null)
            projectileReleaseTransform = transform;
    }

    private void Shoot()
    {
        float projectileSpeed = projectilePrefab.GetComponent<Projectile>().speed;

        Vector3 dir;
        if (accountForBulletDrop)
            dir = Trajectory.Calculate(toPlayer, projectileSpeed);
        else
            dir = toPlayer;

        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Release(dir, projectileReleaseTransform.gameObject);
    }

    private void Shooting()
    {
        if (los)
        {
            if (fireTimer > fireRate)
            {
                fireTimer -= fireRate;
                Shoot();
            }

            fireTimer += Time.deltaTime;
        }
        else
        {
            fireTimer = 0f;
        }
    }

    private void Movement()
    {
        if (los)
        {
            xStrafeDir = 0;
            zStrafeDir = 0;

            bool shouldStrafe = false;
            {
                Vector3 fromPlayer = (transform.position - playerTransform.position).normalized;
                Vector3 playerLook = mainCamera.forward;

                float dot = Vector3.Dot(fromPlayer, playerLook);
                if (dot > 0.85f)
                    shouldStrafe = true;
            }

            if (shouldStrafe)
            {
                int dir = (int)Random.Range(0, 1);
                if (dir == 0)
                    xStrafeDir = -1;
                else
                    xStrafeDir = 1;
            }

            if (distanceToPlayer < closeProximity)
                zStrafeDir = -1;
            else if (distanceToPlayer > farProximity)
                zStrafeDir = 1;

            Vector3 toPlayerPerpendicular = Vector3.Cross(toPlayer, Vector3.up);
            Vector3 relativeTarget = Vector3.zero;

            relativeTarget += toPlayer.normalized*zStrafeDir;
            relativeTarget += toPlayerPerpendicular.normalized*xStrafeDir;

            if (relativeTarget == Vector3.zero)
                ClearPath();
            else
                navMeshAgent.SetDestination(transform.position + relativeTarget);
        }
        else
        {
            PathToPlayer();
        }
    }

    public override void Update()
    {
        base.Update();

        if (enemyStats.isDead)
            return;
        
        if (!aggressive)
            return;

        if (playerStats.isDead) 
            return;

        EvaluateToPlayer();
        CheckLOS();

        Shooting();

        if (decisionTimer >= decisionTickRate)
        {
            Movement();
            decisionTimer -= decisionTickRate;
        }

        decisionTimer += Time.deltaTime;
    }
}
