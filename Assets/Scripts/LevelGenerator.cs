using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator: MonoBehaviour
{
    public int levelSize;
    public int maxRoomSize;
    public int minRoomSize;
    public Tilemap tilemap;
    public TileBase wallTile;
    public TileBase floorTile;
    public TileBase debugTile;

    private TileBase[,] map;

    private class Partition
    {
        public enum Type
        {
            None,
            Horizontal,
            Vertical,
            Room
        }

        public Vector2Int pos;
        public Vector2Int size;
        public Type type;

        public Partition[] children;

        public Partition(Vector2Int pos, Vector2Int size)
        {
            this.pos = pos;
            this.size = size;
            children = new Partition[2];
            type = Type.None;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Initialize map to all walls
        map = new Tile[levelSize, levelSize];
        for (int i = 0; i < levelSize; i++) for (int j = 0; j < levelSize; j++)
                map[i, j] = wallTile;

        // Partition the map into a binary tree
        Partition root = new Partition(new Vector2Int(0, 0), new Vector2Int(levelSize, levelSize));
        Stack<Partition> toProcess = new Stack<Partition>();
        toProcess.Push(root);
        while (toProcess.Count > 0)
        {
            Partition curPartition = toProcess.Pop();
            bool largeX = curPartition.size.x > maxRoomSize + 1;
            bool largeY = curPartition.size.y > maxRoomSize + 1;
            if (!(largeX || largeY))
            {
                // Mark as room
                curPartition.type = Partition.Type.Room;
                // Generate room in map
                for (int i = curPartition.pos.x; i < curPartition.pos.x + curPartition.size.x - 1; i++)
                    for (int j = curPartition.pos.y; j < curPartition.pos.y + curPartition.size.y - 1; j++)
                    {
                        map[i, j] = floorTile;
                    }
            }
            else
            {
                // Divide partition
                Vector2Int[] newPartitionSizes = new Vector2Int[2];
                Vector2Int[] newPartitionPositions = new Vector2Int[2];
                int cut;

                // Ignore the fact that I repeated a ton of code
                if ((largeX && largeY && Random.Range(0f, 1f) > 0.5) || (largeX && !largeY))
                {
                    // Partition vertically
                    curPartition.type = Partition.Type.Vertical;
                    cut = Random.Range(minRoomSize + 1, curPartition.size.x / 2);
                    newPartitionSizes[0] = new Vector2Int(cut, curPartition.size.y);
                    newPartitionSizes[1] = new Vector2Int(curPartition.size.x - cut, curPartition.size.y);
                    newPartitionPositions[0] = curPartition.pos;
                    newPartitionPositions[1] = curPartition.pos + new Vector2Int(cut, 0);
                }
                else
                {
                    // Partition horizontally
                    curPartition.type = Partition.Type.Horizontal;
                    cut = Random.Range(minRoomSize + 1, curPartition.size.y / 2);
                    newPartitionSizes[0] = new Vector2Int(curPartition.size.x, cut);
                    newPartitionSizes[1] = new Vector2Int(curPartition.size.x, curPartition.size.y - cut);
                    newPartitionPositions[0] = curPartition.pos;
                    newPartitionPositions[1] = curPartition.pos + new Vector2Int(0, cut);
                }

                curPartition.children[0] = new Partition(newPartitionPositions[0], newPartitionSizes[0]);
                curPartition.children[1] = new Partition(newPartitionPositions[1], newPartitionSizes[1]);

                // Push new partitions to stack
                toProcess.Push(curPartition.children[0]);
                toProcess.Push(curPartition.children[1]);
            }
        }

        // Debug: generate tilemap
        PopulateTilemap();

        // Connect rooms
        Stack<Partition> stack1 = new Stack<Partition>();
        Stack<Partition> stack2 = new Stack<Partition>();
        stack1.Push(root);
        // Use 2 stacks to reverse order of tree
        while (stack1.Count > 0)
        {
            Partition cur = stack1.Pop();
            if (cur.type != Partition.Type.Room)
            {
                stack2.Push(cur);
                foreach (Partition child in cur.children)
                    if (child.type != Partition.Type.Room)
                        stack1.Push(child);
            }
        }
        while (stack2.Count > 0)
        {
            // Connect children
            Partition cur = stack2.Pop();
            bool[] childIsRoom = cur.children.Select(child => child.type == Partition.Type.Room).ToArray();


            if (childIsRoom[0])
            {
                ConnectRoom(cur.children[0], cur.type, true);
            }
            else if (childIsRoom[1])
            {
                ConnectRoom(cur.children[1], cur.type, false);
            }
            else
            {
                List<Partition> validChildren = GetBorderRooms(cur);
                ConnectRoom(validChildren.ElementAt(Random.Range(0, validChildren.Count())), cur.type, true);
            }
                
        }

        // Generate Tilemap
        PopulateTilemap();
    }

    private void PopulateTilemap()
    {
        tilemap.SetTilesBlock(new BoundsInt(new Vector3Int(), new Vector3Int(levelSize, levelSize, 1)), map.Cast<TileBase>().ToArray());
    }
    
    private void ConnectRoom(Partition room, Partition.Type axis, bool positiveDirection)
    {
        // Mirror vertical case into horizontal case
        bool horizontal = axis == Partition.Type.Horizontal;
        Vector2Int roomSize = room.size;
        Vector2Int roomPos = room.pos;
        if (horizontal)
        {
            Mirror(ref roomSize);
            Mirror(ref roomPos);
        }
        
        int doorXPosition = roomPos.x + (positiveDirection ? (roomSize.x) : 0) - 1;

        var validYPositions = Enumerable.Range(roomPos.y, roomSize.y - 1).Where(
            candidateYPosition =>
            {
                Vector2Int positionToCheck = new Vector2Int(doorXPosition + (positiveDirection ? 1 : -1), candidateYPosition);
                if (horizontal) // Mirror x and y
                    Mirror(ref positionToCheck);

                // Check if tile in front of door is floor
                return map[positionToCheck.x, positionToCheck.y] == floorTile;
            }
        );

 /*       foreach (int pos in Enumerable.Range(roomPos.y, roomSize.y - 1))
        {
            Vector2Int positionToCheck = new Vector2Int(doorXPosition + (positiveDirection ? 1 : -1), pos);
            if (vertical) // Mirror x and y
                Mirror(ref positionToCheck);

            // Check if tile in front of door is floor
            map[positionToCheck.x, positionToCheck.y] = debugTile;
        }
*/

        if (validYPositions.Count() == 0)
        {
            Debug.LogError("Valid door position not found");
        }
        else
        {
            Vector2Int selectedPosition = new Vector2Int(
                doorXPosition,
                validYPositions.ElementAt(Random.Range(0, validYPositions.Count()))
            );
            if (horizontal)
                Mirror(ref selectedPosition);

            map[
                selectedPosition.x,
                selectedPosition.y
            ] = floorTile;
        }
    }

    /**
     * Get a list of rooms which border the partition line.
     */
    private List<Partition> GetBorderRooms(Partition root)
    {
        // Connect a child room which borders the partition line
        Stack<Partition> stack = new Stack<Partition>();
        List<Partition> validChildren = new List<Partition>();
        stack.Push(root.children[0]);
        while (stack.Count > 0)
        {
            Partition cur = stack.Pop();
            Partition[] children = cur.children;
            if (cur.type == root.type) // Both vertical or both horizontal
            {
                if (children[1].type == Partition.Type.Room)
                    validChildren.Add(children[1]);
                else
                    stack.Push(children[1]);
            }
            else // Current partition runs opposite direction to root
            {
                foreach (Partition child in cur.children)
                {
                    if (child.type == Partition.Type.Room)
                        validChildren.Add(child);
                    else
                        stack.Push(child);
                }
            }
        }

        return validChildren;
    }

    private void Mirror(ref Vector2Int vector)
    {
        (vector.x, vector.y) = (vector.y, vector.x);
    }
}
