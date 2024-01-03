using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelPopulator : MonoBehaviour
{
    public Tilemap debugTilemap;
    public Tile debugTile;

    public GameObject playerPrefab;
    public GameObject slimePickupPrefab;
    public GameObject[] enemyPrefabs;

    public EnemyManager enemyManager;

    public GameObject player;

    public void PopulateLevel(int scaleFactor, List<Partition> rooms)
    {
        List<Partition> toProcess = new List<Partition>(rooms);
        Partition startRoom = GetRandomAndRemove(rooms);
        enemyManager.PlayerList = new GameObject[] { PlaceInRoom(playerPrefab, startRoom, scaleFactor) };

        List<GameObject> spawnedEnemies = new List<GameObject>();
        int numRooms = toProcess.Count;
        for (int i = 0; i < numRooms; i++)
        {
            Partition room = GetRandomAndRemove(rooms);
            spawnedEnemies.Add(PlaceInRoom(enemyPrefabs[0], room, scaleFactor));
        }
    }

    private Partition GetRandomAndRemove(List<Partition> list)
    {
        int index = Random.Range(0, list.Count);
        Partition partition = list[index];
        list.RemoveAt(index);
        return partition;
    }

    private GameObject PlaceInRoom(GameObject prefab, Partition room, int scaleFactor)
    {
        Vector2Int scaledPos = new Vector2Int(room.pos.x * scaleFactor, room.pos.y * scaleFactor);
        // Scaled room size (accounting for side wall tiles)
        Vector2Int scaledSize = new Vector2Int(((room.size.x - 1) * scaleFactor) - 1, (room.size.y - 1) * scaleFactor);

        for (int i = scaledPos.x; i < scaledPos.x + scaledSize.x; i++)
            for (int j = scaledPos.y; j < scaledPos.y + scaledSize.y; j++)
                debugTilemap.SetTile(new Vector3Int(j, i, 0), debugTile);

        // Get random position in room
        Vector2Int spawnPosition = new Vector2Int(
            Random.Range(scaledPos.x, scaledPos.x + scaledSize.x),
            Random.Range(scaledPos.y, scaledPos.y + scaledSize.y)
        );

        return GameObject.Instantiate(prefab, new Vector3(spawnPosition.y + 0.5f, spawnPosition.x + 0.5f, -1), Quaternion.identity);
    }
}
