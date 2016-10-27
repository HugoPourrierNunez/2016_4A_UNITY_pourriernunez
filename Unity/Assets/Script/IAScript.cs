using UnityEngine;
using System.Collections;

public class IAScript : MonoBehaviour {

    [SerializeField]
    int analyseDepth = 3;

    GameManagerScript gameManagerScript;

    Node[] nodes;

    Node actualNode;

    private int sizeGameState;


    public static Vector3[] Direction = new Vector3[8]
    {
        new Vector3(0.0f,0.0f, 1.0f),
        new Vector3(0.71f,0.0f, 0.71f),
        new Vector3(1.0f,0.0f, 0.0f),
        new Vector3(-0.71f,0.0f, 0.71f),
        new Vector3(-1.0f,0.0f, 0.0f),
        new Vector3(-0.71f,0.0f, -0.71f),
        new Vector3(0.0f,0.0f, -1.0f),
        new Vector3(0.71f,0.0f, -0.71f)
    };

    public void setGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }


    public Vector3 getNextDirectionIA()
    {
        //tree.val = -1;
        //generateTree(tree,gameManagerScript.ActualGameState, analyseDepth);

        for(var i = 0; i<sizeGameState;i++)
        {
            nodes[i].score = float.MaxValue;
        }

        actualNode.cost = 0;
        actualNode.gameState = gameManagerScript.ActualGameState;

        explore(actualNode, 1, 0);

        /*for (var i = 0; i < sizeGameState; i++)
        {
            if(gameStates[i].score != float.MaxValue)Debug.Log(gameStates[i].score);
        }*/

        return analyse();
    }

    public Vector3 analyse()
    {
        var minScore = float.MaxValue;
        var minVector = Vector3.zero;

        for(var i=0; i<sizeGameState;i++)
        {
            if(nodes[i].score<minScore)
            {
                minScore = nodes[i].score;
                minVector = nodes[i].firstDirection;
            }
        }
        //Debug.Log(minScore);
        return minVector;
    }

    public void explore(Node node, int depth, int index)
    {
        if (depth > analyseDepth)
            return;

        for(var i = 0; i < 8; i++)
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

            nodes[index].score = 1 / nodes[index].gameState.minDistToIA
                + Mathf.Abs(gameManagerScript.MapManagerScript.getGoalTransform().position.x - nodes[index].gameState.iaPosition.x)
                + Mathf.Abs(gameManagerScript.MapManagerScript.getGoalTransform().position.z - nodes[index].gameState.iaPosition.z);

            //Quitte si collision
            if (nodes[index].score == float.MaxValue)
                return;

            nodes[index].cost = depth;

            explore(nodes[index], depth + 1, ++index);
        }
    }


    void Start()
    {
        sizeGameState = (int)Mathf.Pow(8, analyseDepth);
        var nbBombs = gameManagerScript.CollisionManagerScript.getBombManagers().Length;
        nodes = new Node[sizeGameState];
        for(var i=0;i<sizeGameState;i++)
        {
            nodes[i] = new Node(nbBombs);
        }
        actualNode = new Node(nbBombs);
    }
}
