using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelPopulator : MonoBehaviour
{
    public Tilemap debugTilemap;
    public Tile debugTile;

    public GameObject playerPrefab;
    public GameObject slimePickupPrefab;
    public GameObject[] enemyPrefabs;

    public int maxEnemiesPerRoom;
    public float slimePickupChance;

    public EnemyManager enemyManager;

    public void PopulateLevel(int scaleFactor, List<Partition> rooms)
    {
        List<Partition> toProcess = new List<Partition>(rooms);
        Partition startRoom = GetRandomAndRemove(toProcess);
        // Spawn mr. player
        enemyManager.PlayerList = new GameObject[] { PlaceInRoom(playerPrefab, startRoom, scaleFactor, -1, new HashSet<Vector2Int>()) };

        List<GameObject> spawnedEnemies = new List<GameObject>();
        foreach (var _ in Enumerable.Range(0, toProcess.Count))
        {
            Partition room = GetRandomAndRemove(toProcess);
            HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
            
            // Spawn up to maxEnemiesPerRoom enemies
            foreach (var __ in Enumerable.Range(0, Random.Range(0, maxEnemiesPerRoom)))
                spawnedEnemies.Add(PlaceInRoom(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], room, scaleFactor, -1, occupied));

            // Chance to spawn a pickup
            if (Random.value < slimePickupChance)
                PlaceInRoom(slimePickupPrefab, room, scaleFactor, -0.5f, occupied);
        }
    }

    private Partition GetRandomAndRemove(List<Partition> list)
    {
        int index = Random.Range(0, list.Count);
        Partition partition = list[index];
        list.RemoveAt(index);
        return partition;
    }

    private GameObject PlaceInRoom(GameObject prefab, Partition room, int scaleFactor, float zPos, HashSet<Vector2Int> occupied)
    {
        Vector2Int scaledPos = new Vector2Int(room.pos.x * scaleFactor, room.pos.y * scaleFactor);
        // Scaled room size (accounting for side wall tiles)
        Vector2Int scaledSize = new Vector2Int(((room.size.x - 1) * scaleFactor) - 1, (room.size.y - 1) * scaleFactor);

        /*        for (int i = scaledPos.x; i < scaledPos.x + scaledSize.x; i++)
                    for (int j = scaledPos.y; j < scaledPos.y + scaledSize.y; j++)
                        debugTilemap.SetTile(new Vector3Int(j, i, 0), debugTile);*/

        // Get random position in room
        Vector2Int spawnPosition;
        do
        {
            spawnPosition = new Vector2Int(
                Random.Range(scaledPos.x, scaledPos.x + scaledSize.x),
                Random.Range(scaledPos.y, scaledPos.y + scaledSize.y)
            );
        } while (occupied.Contains(spawnPosition));
        occupied.Add(spawnPosition);
        
        return GameObject.Instantiate(prefab, new Vector3(spawnPosition.y, spawnPosition.x, zPos), Quaternion.identity);
    }
}
