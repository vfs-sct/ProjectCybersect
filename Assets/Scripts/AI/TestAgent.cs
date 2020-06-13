using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestAgent : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    private NavMeshAgent agent;
    private UtilitySelector utilitySelector;
    private Transform player;
    private Transform mainCamera;

    private bool pathedToPlayer = false;
    private bool los = false;
    private Vector3 toPlayer = Vector3.zero;
    private float toPlayerSqrMag = 0f;
    private Random random = new Random();

    private float shootTimer = 0f;
    private float shootTime = 0.75f;

    private void EvalutateToPlayer()
    {
        toPlayer = player.position - transform.position;
        toPlayerSqrMag = toPlayer.sqrMagnitude;
    }

    private void CheckLOS()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, toPlayer, out hit))
        {
            if (hit.transform == player.transform)
                los = true;
            else
                los = false;

            return;
        }

        los = false;
    }

    private float MoveToPlayerUtility()
    {
        float utility = 0f;

        if (los && toPlayerSqrMag > 14*14f)
        {
            utility = Mathf.Clamp01(toPlayerSqrMag/(30f*30f))*40f;
        }

        return utility;
    }

    private float MoveAwayPlayerUtility()
    {
        float utility = 0f;

        if (los && toPlayerSqrMag < 10*10f)
        {
            utility = (1 - Mathf.Clamp01(toPlayerSqrMag/(5*5f)))*40f;
        }

        return utility;
    }

    private float GetLineOfSightUtility()
    {
        float utility = 0f;

        if (!los)
            utility = 120f;

        return utility;
    }

    private float StrafeUtility()
    {
        float utility = 0f;

        Vector3 fromPlayer = (transform.position - player.position).normalized;
        Vector3 playerLook = mainCamera.forward;

        float dot = Vector3.Dot(fromPlayer, playerLook);
        if (dot > 0.85f)
            utility = dot*60f;

        return utility;
    }

    private void Strafe()
    {
        if (agent.hasPath && !pathedToPlayer)
            return;

        Vector3 perpendicular = Vector3.Cross(toPlayer, Vector3.up);
        perpendicular.Normalize();

        float rnd = Random.Range(-1f, 1f);
        perpendicular *= Mathf.Sign(rnd);

        agent.SetDestination(transform.position + perpendicular*2f);

        pathedToPlayer = false;
    }

    private void PathToPlayer()
    {
        agent.SetDestination(player.position);
        pathedToPlayer = true;
    }

    private void PathAwayFromPlayer()
    {
        Vector3 toPlayerNormalize = toPlayer.normalized;
        toPlayerNormalize *= 3f;
        agent.SetDestination(transform.position - toPlayerNormalize);
    }

    private void ClearPath()
    {
        agent.ResetPath();
        pathedToPlayer = false;
    }

    private void InitUtilitySelector()
    {
        utilitySelector = new UtilitySelector();

        Selection.Input losInput = GetLineOfSightUtility;
        Selection.Callback losCallback = PathToPlayer;
        utilitySelector.AddSelection(losInput, losCallback);

        Selection.Input strafeInput = StrafeUtility;
        Selection.Callback strafeCallback = Strafe;
        utilitySelector.AddSelection(strafeInput, strafeCallback);

        Selection.Input toPlayerInput = MoveToPlayerUtility;
        Selection.Callback toPlayerCallback = PathToPlayer;
        utilitySelector.AddSelection(toPlayerInput, toPlayerCallback);

        Selection.Input awayPlayerInput = MoveAwayPlayerUtility;
        Selection.Callback awayPlayerCallback = PathAwayFromPlayer;
        utilitySelector.AddSelection(awayPlayerInput, awayPlayerCallback);

        utilitySelector.SetDefault(ClearPath);
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("player").transform;
        mainCamera = GameObject.Find("mainCamera").transform;

        InitUtilitySelector();
    }

    private void Shoot()
    {
        float projectileSpeed = projectilePrefab.GetComponent<Projectile>().speed;
        Vector3 dir = Trajectory.Calculate(toPlayer, projectileSpeed);
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Release(dir, gameObject);
    }

    private void Shooting()
    {
        if (los)
        {
            if (shootTimer > shootTime)
            {
                shootTimer -= shootTime;
                Shoot();
            }

            shootTimer += Time.deltaTime;
        }
        else
        {
            shootTimer = 0f;
        }
    }

    float timer = 0f;
    private void Update()
    {
        if (!AIState.aggressive)
            return;

        if (timer > AI.decisionTickPeriod)
        {
            timer -= AI.decisionTickPeriod;

            EvalutateToPlayer();
            CheckLOS();
            utilitySelector.Run();
        }

        timer += Time.deltaTime;

        Shooting();
    }
}
