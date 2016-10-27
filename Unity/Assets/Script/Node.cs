using UnityEngine;
using System.Collections;

public class Node {

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
