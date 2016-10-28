using UnityEngine;
using System.Collections;
using System.Threading;

public class IAWithAStarScript : MonoBehaviour
{
    [SerializeField]
    private int analyseDepth = 3;

    [SerializeField]
    LongTermScript longTermScript;

    GameManagerScript gameManagerScript;
    Node[] nodes;
    Node actualNode;
    int sizeGameState;
    bool longTermPlanned = false;
    Vector3[] checkPoints;
    int reachedCheckPoint = 0;


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

        StartCoroutine(GetCheckPoints());
        /*MultiThreadingScript mts = new MultiThreadingScript(new IACallBack(ResultCallback), longTermScript, gameManagerScript.actualGameState.iaPosition, gameManagerScript.MapManagerScript.GetGoalTransform().position);

        Thread t = new Thread(new ThreadStart(mts.GetCheckPoints));
        t.Start();
        t.Join();*/
    }

    IEnumerator GetCheckPoints()
    {
        checkPoints = longTermScript.FindAICheckPoints(gameManagerScript.actualGameState.iaPosition, gameManagerScript.MapManagerScript.GetGoalTransform().position);
        for(var i = 0; i < checkPoints.Length; i++)
        {
            Debug.Log(checkPoints[i].x + "/" + checkPoints[i].z);
        }
        yield return new WaitForSeconds(1.0f);
        longTermPlanned = true;
    }

    /*
    public void ResultCallback(Vector3[] checkPoints)
    {
        longTermPlanned = true;
        this.checkPoints = checkPoints;
    }*/

    public void SetGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    public Vector3 GetNextDirectionIA()
    {
        if(!longTermPlanned)
        {
            return Vector3.zero;
        }

        for(var i = 0; i < sizeGameState; i++)
        {
            nodes[i].score = float.MaxValue;
        }

        actualNode.cost = 0;
        actualNode.gameState = gameManagerScript.ActualGameState;

        Debug.Log(actualNode.gameState.iaPosition.x + "/" + actualNode.gameState.iaPosition.z);

        if (actualNode.gameState.iaPosition.x + gameManagerScript.CollisionManagerScript.iaRadius > checkPoints[reachedCheckPoint].x
            && actualNode.gameState.iaPosition.x - gameManagerScript.CollisionManagerScript.iaRadius < checkPoints[reachedCheckPoint].x
            && actualNode.gameState.iaPosition.z + gameManagerScript.CollisionManagerScript.iaRadius > checkPoints[reachedCheckPoint].z
            && actualNode.gameState.iaPosition.z - gameManagerScript.CollisionManagerScript.iaRadius < checkPoints[reachedCheckPoint].z)
        {
            //Debug.Log(actualNode.gameState.iaPosition.x + "/" + actualNode.gameState.iaPosition.z);
            reachedCheckPoint++;
        }

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

            nodes[index].score = Mathf.Abs(checkPoints[reachedCheckPoint].x - nodes[index].gameState.iaPosition.x)
                + Mathf.Abs(checkPoints[reachedCheckPoint].z - nodes[index].gameState.iaPosition.z);

            if (gameManagerScript.actualGameState.bombs.Length > 0)
            {
                nodes[index].score += 1 / nodes[index].gameState.minDistToIA;
            }

            if (nodes[index].score == float.MaxValue)
                return;

            nodes[index].cost = depth;

            Explore(nodes[index], depth + 1, ++index);
        }
    }
}
