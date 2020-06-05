using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FSM : MonoBehaviour {
    private LineOfSight  checkMyVision;
    private NavMeshAgent agent;
    private Transform playerTransform;

    private Transform patrolDestination;

    private Health playerHealth;

    public float maxDamage = 10f;

    // Enums to keep states
    public enum ENEMY_STATES { patrol, chase, attack }

    // We need a property to get the current state
    [SerializeField]
    private ENEMY_STATES currentState;
    public ENEMY_STATES CurrentState {
        get { return currentState; }
        set {
            currentState = value;
            StopAllCoroutines ();
            switch (currentState) {
                case ENEMY_STATES.patrol:
                    StartCoroutine (EnemyPatrol ());
                    break;
                case ENEMY_STATES.chase:
                    StartCoroutine (EnemyChase ());
                    break;
                case ENEMY_STATES.attack:
                    StartCoroutine (EnemyAttack ());
                    break;
            }
        }
    }

    private void Awake () {
        checkMyVision = GetComponent<LineOfSight > ();
        agent = GetComponent<NavMeshAgent> ();
        playerHealth = GameObject. FindGameObjectWithTag("Player").GetComponent<Health> ();
        playerTransform = playerHealth.GetComponent<Transform> ();
    }
    // Start is called before the first frame update
    void Start () {

        // GameObject[] destinations = GameObject.FindGameObjectsWithTag ("Dest");
        // int pathIndex = Random.Range (0, destinations.Length);
        patrolDestination = GameObject.FindGameObjectWithTag("Dest").GetComponent<Transform>();
        //  print($"Path: {pathIndex}");
        CurrentState = ENEMY_STATES.patrol;

    }

    public IEnumerator EnemyPatrol () {
        print ("Patroling");
        while (currentState == ENEMY_STATES.patrol) {
            agent.speed = 8;
            checkMyVision.sensitivity = LineOfSight.Sensitivity.HIGH;
            agent.isStopped = false;
            agent.SetDestination (patrolDestination.position);
            while (agent.pathPending) {

                yield return null;
            }
            while (checkMyVision.targetInSight) {
                agent.isStopped = true;
                print ("Patrol -> Chasing  ");
                CurrentState = ENEMY_STATES.chase;
                yield break;
            }

            yield return null;
        }

    }
    public IEnumerator EnemyChase () {
        print ("Chasing");
        while (currentState == ENEMY_STATES.chase) {
            checkMyVision.sensitivity = LineOfSight.Sensitivity.LOW;
            agent.isStopped = false;
            // agent.acceleration = 600;
            // agent.speed = 250;
            agent.ResetPath ();
            // agent.CalculatePath (checkMyVision.lastKnownSighting, agent.path);
            bool destSet = agent.SetDestination (checkMyVision.lastknownSight);
            bool pending = agent.pathPending;
            while (agent.pathPending) {
                yield return null;
            }
            print ($"Path Pending: {agent.pathPending}");
            if (agent.remainingDistance <= agent.stoppingDistance) {
                agent.isStopped = true;
                // print ($"Target In Sight for Chase ? {checkMyVision.targetInSight} ");
                if (!checkMyVision.targetInSight) {
                    print ("Chasing -> Patrol");
                    CurrentState = ENEMY_STATES.patrol;
                } else {
                    // print ("Sqwitching to Attack!!!!!");
                    print ("Chasing -> Attack");
                    CurrentState = ENEMY_STATES.attack;
                }
                yield break;
            }
            yield return null;
        }
        // agent.acceleration = 600;
        // print(checkMyVision.lastKnownSighting);
        // print("Dest Set: " + destSet);
        //   print("Path Invalid: " + (agent.path.status == NavMeshPathStatus.PathInvalid));
        // print("Agent Remaining Distance: " + agent.remainingDistance);
        //  print("Agent Stopping Distance: " + agent.stoppingDistance);
    }
    public IEnumerator EnemyAttack () {
        print ("Attacking enemy");
        while (currentState == ENEMY_STATES.attack) {
            agent.isStopped = false;
            // agent.ResetPath();
            agent.SetDestination (playerTransform.position);
            while (agent.pathPending) {

                yield return null;
            }
            if (agent.remainingDistance > agent.stoppingDistance) {
                print ("Attack -> Chasing");
                CurrentState = ENEMY_STATES.chase;
                yield break;
            } else {
                // Do something

                playerHealth.healthpoints -= maxDamage * Time.deltaTime;
            }
            yield return null;
        }

        yield break;
    }

    // Update is called once per frame
    void Update () {

    }
}