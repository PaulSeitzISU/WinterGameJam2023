using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partition
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
    public bool largeRoom;

    public Partition[] children;

    public Partition(Vector2Int pos, Vector2Int size)
    {
        this.pos = pos;
        this.size = size;
        largeRoom = false;
        children = new Partition[2];
        type = Type.None;
    }
}
