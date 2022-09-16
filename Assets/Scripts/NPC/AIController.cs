using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private float startWaitTime = 4f;
    private float startTimeToRotate = 2f;
    [SerializeField] private float speedWalk = 6f;
    [SerializeField] private float speedRun = 9f;

    public float health = 123;
    public int damage = 5;
    private bool attacking = false;
    [SerializeField] private GameObject attackEffect;
    [SerializeField] private GameObject attackTrigger;
    private Vector3 startPosition;

    private float vievRadius = 15f;
    private float vievAngle = 90f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private Transform[] waypointsList;
    private int currentWaypointIndex;

    private Vector3 playerLastPosition = Vector3.zero;
    private Vector3 playerPosition;

    private float waitTime;
    private float timeToRotate;
    private bool playerInRange;
    private bool playerNear;
    private bool isPatrol;
    private void Start()
    {
        playerPosition = Vector3.zero;
        isPatrol = true;
        playerInRange = false;
        waitTime = startWaitTime;
        timeToRotate = startTimeToRotate;
        startPosition = transform.position;

        navMeshAgent = GetComponent<NavMeshAgent>();
        Move(speedWalk);

        currentWaypointIndex = 0;
        navMeshAgent.SetDestination(waypointsList[currentWaypointIndex].position);
    }

    private void Update()
    {
        EnviromentViev();

        if (!isPatrol)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
    }

    private void EnviromentViev()
    {
        Collider[] playerColliders = Physics.OverlapSphere(transform.position, vievRadius, playerMask);

        for (int i = 0; i < playerColliders.Length; i++)
        {
            Transform player = playerColliders[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            if ((Vector3.Angle(transform.forward, dirToPlayer) < vievAngle / 2) || Vector3.Distance(transform.position, player.position) <= 3f)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    playerInRange = true;
                    isPatrol = false;
                }
                else
                {
                    playerInRange = false;
                }
            }

            if (Vector3.Distance(transform.position, player.position) > vievRadius)
            {
                playerInRange = false;
            }

            if (playerInRange)
            {
                playerPosition = player.transform.position;
            }
        }
    }

    private void Chase()
    {
        playerNear = false;
        playerLastPosition = Vector3.zero;

        Move(speedRun);
        navMeshAgent.SetDestination(playerPosition);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !attacking)
        {  
            StartCoroutine(Attack());
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (waitTime <= 0 && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                isPatrol = true;
                playerNear = false;
                Move(speedWalk);
                waitTime = startWaitTime;
                navMeshAgent.SetDestination(waypointsList[currentWaypointIndex].position);
            }
            else
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2f)
                {
                    Stop();
                    waitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Patrol()
    {
        if (playerNear)
        {
            if (timeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                Stop();
                timeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            playerNear = false;
            playerLastPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypointsList[currentWaypointIndex].position);

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (waitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    waitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    waitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void NextPoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypointsList.Length;
        navMeshAgent.SetDestination(waypointsList[currentWaypointIndex].position);
    }

    private void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    private void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    private void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);

        if (Vector3.Distance(transform.position, player) >= 2f)
        {
            if (waitTime <= 0)
            {
                playerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypointsList[currentWaypointIndex].position);
                waitTime = startWaitTime;
                timeToRotate = startTimeToRotate;
            }
            else
            {
                Stop();
                waitTime -= Time.deltaTime;
            }
        }
    }

    private IEnumerator Attack()
    {
        attacking = true;
        attackTrigger.SetActive(true);
        attackEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        attackEffect.SetActive(false);
        attackTrigger.SetActive(false);
        attacking = false;
    }

    public void RecieveDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            gameObject.transform.position = startPosition;
            health = 123;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RecieveDamage(other.GetComponent<BattleModule>().damage);
        }
    }
}
