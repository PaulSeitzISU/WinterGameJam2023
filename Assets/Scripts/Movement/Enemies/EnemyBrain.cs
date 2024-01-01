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
    public EnemyState currentState { get; private set; }

    // Reference to movement script, grid manager, and other necessary components
    private Health health;
    private Movement movement;
    private GridManager gridManager;
    private Tilemap tilemap;
    public int visibilityRadius;
    public List<GameObject> objectsInRadius;

    // Other variables necessary for the enemy's behavior
    public float patrolSpeed = 3.0f;
    public float chaseSpeed = 5.0f;
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
        CheckForGameObjects();
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

    private void CheckForGameObjects()
{
    objectsInRadius = gridManager.GetObjectsInRadius(movement.currentGridPosition, visibilityRadius, gameObject);

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
                //Debug.Log("Player detected at corner");
                TransitionToState(EnemyState.Chase);
            }
        }
        //check if the player is next to the enemy
        foreach(GameObject player in PlayerList)
        {
            if (player != null)
            {
                if (Vector3.Distance(player.transform.position, transform.position) > attackDistanceMin)
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

}
