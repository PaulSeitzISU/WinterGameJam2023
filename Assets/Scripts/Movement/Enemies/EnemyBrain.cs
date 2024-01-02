using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Flee,
    Dead
}

public class EnemyBrain : MonoBehaviour
{
     [SerializeField] public EnemyState currentState;

    // Reference to movement script, grid manager, and other necessary components
    private Health health;
    private Movement movement;
    private GridManager gridManager;
    private Tilemap tilemap;
    public int visibilityRadius;
    public int visibilityRadiusFar;
    public List<GameObject> objectsInRadius;

    // Other variables necessary for the enemy's behavior
    public int patrolSpeed = 3;
    public int chaseSpeed = 5;
    public float attackDistance = 1.5f;
    public float attackDistanceMin = -1f;

    public float fleeDistance = 8.0f;

    public List<GameObject> PlayerList = new List<GameObject>();

    [SerializeField] public EnemyState StartState;

    [SerializeField] private UnityEvent OnIdle;
    [SerializeField] private UnityEvent OnPatrol;
    [SerializeField] private UnityEvent OnChase;
    [SerializeField] private UnityEvent OnAttack;
    [SerializeField] private UnityEvent OnFlee;
    [SerializeField] private UnityEvent OnDead;


    void Start()
    {
        currentState = StartState;
        // Get references to necessary components
        movement = GetComponent<Movement>();
        health = GetComponent<Health>();
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Find the GridManager in the scene
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>(); // Get the Tilemap component
    }

    private void Update()
    {
        //TakeTurn();
        
    }

    public void TakeTurn()
    {
        CheckIfDead();
        if (currentState == EnemyState.Dead)
        {
            return;
        }
        CheckForGameObjects(visibilityRadius);
        UpdateState();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                // Define behavior for Idle state
                Idle();
                break;
            case EnemyState.Patrol:
                // Define behavior for Patrol state
                Patrol();
                break;
            case EnemyState.Chase:
                // Define behavior for Chase state
                Chase();
                break;
            case EnemyState.Attack:
                // Define behavior for Attack state
                Attack();
                break;
            case EnemyState.Flee:
                // Define behavior for Flee state
                Flee();
                break;
            case EnemyState.Dead:
                // Define behavior for Dead state
                Dead();
                break;
        }
    }

    // Transition between states based on certain conditions
    private void TransitionToState(EnemyState newState)
    {
        currentState = newState;
    }

    private void Idle()
    {
        // Implement idle behavior here    

        OnIdle.Invoke();
    }

    private void Patrol()
    {
        // Implement patrol behavior here
        // Move the enemy in a predefined patrol pattern using the Movement script
        OnPatrol.Invoke();
    }

    private void Chase()
    {
        // Implement chase behavior here
        // Move the enemy towards the target (player or specific position) using the Movement script

        OnChase.Invoke();
    }

    private void Attack()
    {
        // Implement attack behavior here
        // Perform attack actions on the target (player or other game objects)

        OnAttack.Invoke();
    }

    private void Flee()
    {
        // Implement flee behavior here
        // Move the enemy away from the target (player or specific position) using the Movement script
        // Example: movement.MoveToGrid(targetGridPosition);

        OnFlee.Invoke();
    }

    private void Dead()
    {
        // Implement death behavior here
        // Handle enemy's death actions

        OnDead.Invoke();
    }
    public GameObject CheckForGameObjectsFar()
    {
        List<GameObject> objectsInRadiusTemp = gridManager.GetObjectsInRadius(movement.currentGridPosition, visibilityRadiusFar, gameObject);

        if (objectsInRadiusTemp.Count > 0)
        {
            GameObject closestPlayer = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject obj in objectsInRadiusTemp)
            {
                if (obj.tag == "Player")
                {
                    // Directly add the player to the list without checking duplicates
                    // You can remove duplicates later if necessary
                    Debug.Log("Player detected at corner" + gameObject.name);

                    float distance = Vector3.Distance(transform.position, obj.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayer = obj;
                    }
                }
            }

            // Remove duplicates from PlayerListTempList if needed
            // PlayerListTempList = PlayerListTempList.Distinct().ToList();

            //Debug.Log("Closest player is " + closestPlayer?.name);
            return closestPlayer;
        }
        return null;
    }


    private void CheckForGameObjects(int Radius)
    {
        objectsInRadius = gridManager.GetObjectsInRadius(movement.currentGridPosition, Radius, gameObject);

        if (objectsInRadius.Count > 0)
        {
            //Debug.Log("Objects detected at corner: " + objectsInRadius.Count);
            foreach (GameObject obj in objectsInRadius)
            {
                if(obj.tag == "Player")
                {
                    if(!PlayerList.Contains(obj))
                    {
                        PlayerList.Add(obj);
                    }
                    Debug.Log("Player detected at corner" + gameObject.name);
                    TransitionToState(EnemyState.Chase);
                }
            }
            //check if the player is next to the enemy
            foreach(GameObject player in PlayerList)
            {
                if (player != null)
                {
                    if (Vector3.Distance(player.transform.position, transform.position) < attackDistanceMin)
                    {
                        TransitionToState(EnemyState.Flee);
                    }
                    else if (Vector3.Distance(player.transform.position, transform.position) < attackDistance)
                    {
                        TransitionToState(EnemyState.Attack);
                    }
                }
            }
        }
    }

    public GameObject ClosestPlayer()
    {
                // Find closest player
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in PlayerList)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }
        //Debug.Log("Closest player is " + closestPlayer.name);
        return closestPlayer;
    }
    //check if dead
    public void CheckIfDead()
    {
        if (health.currentHealth <= 0)
        {
            Debug.Log("Player is dead");
            TransitionToState(EnemyState.Dead);
        }
    }

}
