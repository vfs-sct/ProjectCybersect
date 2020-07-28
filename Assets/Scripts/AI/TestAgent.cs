using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestAgent : AI
{
    public bool accountForBulletDrop = true;
    public float fireRate = 0.75f;
    public GameObject projectilePrefab;

    private NavMeshAgent navMeshAgent;
    private PlayerStats playerStats;
    private Transform playerTransform;
    private Transform mainCamera;
    private EnemyStats enemyStats;

    private bool pathedToPlayer = false;
    private bool los = false;
    private Vector3 toPlayer = Vector3.zero;
    private float toPlayerSqrMag = 0f;
    private Random random = new Random();

    private float fireTimer = 0f;

    private void EvalutateToPlayer()
    {
        toPlayer = playerTransform.position - transform.position;
        toPlayerSqrMag = toPlayer.sqrMagnitude;
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

    private float StrafeUtility()
    {
        float utility = 0f;

        Vector3 fromPlayer = (transform.position - playerTransform.position).normalized;
        Vector3 playerLook = mainCamera.forward;

        float dot = Vector3.Dot(fromPlayer, playerLook);
        if (dot > 0.85f)
            utility = dot*60f;

        return utility;
    }

    private void Strafe()
    {
        if (navMeshAgent.hasPath && !pathedToPlayer)
            return;

        Vector3 perpendicular = Vector3.Cross(toPlayer, Vector3.up);
        perpendicular.Normalize();

        float rnd = Random.Range(-1f, 1f);
        perpendicular *= Mathf.Sign(rnd);

        navMeshAgent.SetDestination(transform.position + perpendicular*2f);

        pathedToPlayer = false;
    }

    private void PathToPlayer()
    {
        navMeshAgent.SetDestination(playerTransform.position);
        pathedToPlayer = true;
    }

    private void PathAwayFromPlayer()
    {
        Vector3 toPlayerNormalize = toPlayer.normalized;
        toPlayerNormalize *= 3f;
        navMeshAgent.SetDestination(transform.position - toPlayerNormalize);
    }

    private void ClearPath()
    {
        navMeshAgent.ResetPath();
        pathedToPlayer = false;
    }

    new private void Start()
    {
        base.Start();

        navMeshAgent = GetComponent<NavMeshAgent>();
        playerStats = GameObject.Find("player").GetComponent<PlayerStats>();
        playerTransform = GameObject.Find("player").transform;
        mainCamera = GameObject.Find("mainCamera").transform;
        enemyStats = GetComponent<EnemyStats>();
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
        projectile.Release(dir, gameObject);
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

    float timer = 0f;
    new private void Update()
    {
        base.Update();

        if (enemyStats.isDead)
            return;
        
        if (!aggressive)
            return;

        if (playerStats.isDead) 
            return;

        Shooting();
    }
}
