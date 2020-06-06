using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_FSM : MonoBehaviour
{
    public enum ENEMY_STATE { PATROL, CHASE, ATTACK };

    [SerializeField]
    private ENEMY_STATE currentState;
    private CheckMyVision checkMyVision;

    private NavMeshAgent agent = null;

    private Transform playerTransform = null;

    private Transform destinationPatrol = null;

    private Health playerHealth;

    public float maxDamage = 10f;

    public ENEMY_STATE CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;

            StopAllCoroutines();
            switch (currentState)
            {
                case ENEMY_STATE.PATROL:
                    StartCoroutine(EnemyPatrol());
                    break;
                case ENEMY_STATE.CHASE:
                    StartCoroutine(EnemyChase());
                    break;
                case ENEMY_STATE.ATTACK:
                    StartCoroutine(EnemyAttack());
                    break;
            }

        }
    }

    public IEnumerator EnemyPatrol()
    {
        while ((CurrentState == ENEMY_STATE.PATROL))
        {
            checkMyVision.sensitivity = CheckMyVision.enmSensitivity.HIGH;
            agent.isStopped = false;
            agent.SetDestination(destinationPatrol.position);
            while (agent.pathPending)
            {
                yield return null;
            }

            if (checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                Debug.Log("Chasing");
                currentState = ENEMY_STATE.CHASE;
                yield break;
            }
        }

        yield break;
    }

    public IEnumerator EnemyChase()
    {
        while ((CurrentState == ENEMY_STATE.CHASE))
        {
            checkMyVision.sensitivity = CheckMyVision.enmSensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastKnownSighting);
            while (agent.pathPending)
            {
                yield return null;
            }

            if (checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                currentState = ENEMY_STATE.CHASE;
                yield break;
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.isStopped = true;
            }

            if (!checkMyVision.targetInSight)
            {
                currentState = ENEMY_STATE.PATROL;
            }
            else
            {
                currentState = ENEMY_STATE.ATTACK;
            }
            yield break;
        }

        yield return null;
    }

    public IEnumerator EnemyAttack()
    {
        while (currentState == ENEMY_STATE.ATTACK)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            while (agent.pathPending)
            {
                yield return null;

                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    currentState = ENEMY_STATE.CHASE;
                }
                else
                {
                    playerHealth.HealthPoints -= maxDamage * Time.deltaTime;
                }

                yield return null;
            }
            yield break;
        }
        yield break;
    }

    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerTransform = playerHealth.GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = ENEMY_STATE.PATROL;
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
        destinationPatrol = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
