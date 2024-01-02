using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelPopulator : MonoBehaviour
{
    public Vector2Int startPosition;
    public GameObject playerPrefab;
    public GameObject slimePickupPrefab;
    public GameObject enemies;

    private EnemyManager enemyManager;

    public void PopulateLevel(int levelSize, Tilemap tilemap)
    {

    }
}
