using UnityEngine;
using System.Collections;

public class IAScript : MonoBehaviour
{
    [SerializeField]
    private int analyseDepth = 3;

    [SerializeField]
    GameManagerScript gameManagerScript;

    Node[] nodes;
    Node actualNode;
    int sizeGameState;


    public static Vector3[] Direction = new Vector3[8]
    {
        new Vector3(0.0f,0.0f, 1.0f), //0
        new Vector3(0.71f,0.0f, 0.71f), //1
        new Vector3(1.0f,0.0f, 0.0f), //2
        new Vector3(-0.71f,0.0f, 0.71f), //3

        new Vector3(0.0f,0.0f, -1.0f), //4
        new Vector3(0.71f,0.0f, -0.71f), //5
        new Vector3(-1.0f,0.0f, 0.0f), //6
        new Vector3(-0.71f,0.0f, -0.71f) //7
    };

    public static int[] InvDirection= new int[32]
        {
           4,0,0,0,
           5,3,0,0,
           0,6,0,0,
           7,0,0,1,
           0,0,0,0,
           0,7,1,0,
           0,0,0,2,
           0,0,3,5

        };

    void Start()
    {
        var nbBombs = gameManagerScript.CollisionManagerScript.getBombManagers().Length;
        sizeGameState = (int)Mathf.Pow(8, analyseDepth);
        nodes = new Node[sizeGameState];

        for (var i = 0; i < sizeGameState; i++)
        {
            nodes[i] = new Node(nbBombs);
        }
        actualNode = new Node(nbBombs);
    }

    public void SetGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    public Vector3 GetNextDirectionIA()
    {
        for(var i = 0; i < sizeGameState; i++)
        {
            nodes[i].score = float.MaxValue;
        }

        actualNode.cost = 0;
        actualNode.gameState = gameManagerScript.ActualGameState;

        Explore(actualNode, 1, 0);

        return Analyse();
    }

    public Vector3 Analyse()
    {
        var minScore = float.MaxValue;
        var minVector = Vector3.zero;

        for(var i = 0; i < sizeGameState; i++)
        {
            if(nodes[i].score < minScore)
            {
                minScore = nodes[i].score;
                minVector = nodes[i].firstDirection;
            }
        }

        return minVector;
    }

    public void Explore(Node node, int depth, int index)
    {
        if (depth > analyseDepth)
            return;

        for (var i = 0; i < 8; i++)
        {
            if (depth == 1)
            {
                nodes[index].firstDirection = Direction[i];
            }
            else
            {
                nodes[index].firstDirection = node.firstDirection;
            }

            nodes[index].isClosed = false;

            gameManagerScript.CollisionManagerScript.FillNextGameState(node.gameState, nodes[index].gameState, Direction[i]);

            if (nodes[index].gameState.minDistToIA == 0)
            {
                //nodes[index].score = 
                continue;
            }
                

            nodes[index].score = 1 / nodes[index].gameState.minDistToIA
                + Mathf.Abs(gameManagerScript.MapManagerScript.GetGoalTransform().position.x - nodes[index].gameState.iaPosition.x)
                + Mathf.Abs(gameManagerScript.MapManagerScript.GetGoalTransform().position.z - nodes[index].gameState.iaPosition.z);

            nodes[index].cost = depth;

            Explore(nodes[index], depth + 1, ++index);
        }
    }
}
