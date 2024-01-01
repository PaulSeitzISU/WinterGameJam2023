using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAIChase : MonoBehaviour
{
    public Movement movement;
    public GridManager gridManager;
    public EnemyBrain enemyBrain;
    public Tilemap tilemap;

    void Start()
    {
        movement = GetComponent<Movement>();
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        enemyBrain = GetComponent<EnemyBrain>();
    }

    public void Chase()
    {
        // Find closest player
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in enemyBrain.PlayerList)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null)
        {
            Vector3Int playerGridPos = tilemap.WorldToCell(closestPlayer.transform.position);
            Vector3Int currentGridPos = tilemap.WorldToCell(transform.position);

            Vector3Int direction = new Vector3Int(
                Mathf.RoundToInt(Mathf.Sign(playerGridPos.x - currentGridPos.x)),
                Mathf.RoundToInt(Mathf.Sign(playerGridPos.y - currentGridPos.y)),
                0);

            Vector3Int targetGridPos = currentGridPos + direction;
            Vector3 targetWorldPos = tilemap.GetCellCenterWorld(targetGridPos);

            movement.MoveToGrid(targetGridPos);
        }
    }
}
