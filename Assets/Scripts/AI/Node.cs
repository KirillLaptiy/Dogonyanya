using UnityEngine;

public class Node
{
    public Vector3Int pos;
    public int gCost = int.MaxValue;
    public int hCost;
    public Node parent;

    public Node(Vector3Int pos) { this.pos = pos; }
    public int fCost { get { return gCost + hCost; } }
}
