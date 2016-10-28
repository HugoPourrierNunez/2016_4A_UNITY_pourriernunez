using UnityEngine;
using System.Collections;

public class Node
{

    public GameState gameState;
    public float cost;
    public bool isClosed;
    public float score;
    public Vector3 firstDirection;

    public Node(int nbBomb)
    {
        cost = 0;
        isClosed = false;
        score = float.MaxValue;
        firstDirection = Vector3.zero;
        gameState = new GameState(nbBomb);
    }
}

public class LongTermNode
{
    /*
    public Vector3 iAPosition;
    public float cost;
    public float score;
    public bool isClosed;
    public LongTermNode prevNode;
    public LongTermNode nextNode;

    public LongTermNode(int nbBomb)
    {
        iAPosition = Vector3.zero;
        cost = 0;
        score = float.MaxValue;
        isClosed = false;
        prevNode = null;
        nextNode = null;
    }
    */

    public bool walkable = true;
    public bool closed = false;
    public bool visited = false;
    public Vector3 worldPosition;
    public int gridX;
    public int gridZ;

    public int cost;
    public int weight;
    public LongTermNode parent;

    public LongTermNode(bool walkable, Vector3 worldPos, int gridX, int gridZ)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridX = gridX;
        this.gridZ = gridZ;
    }

    public int Score
    {
        get
        {
            return cost + weight;
        }
    }
}
