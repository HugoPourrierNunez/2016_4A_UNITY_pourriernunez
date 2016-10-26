using UnityEngine;
using System.Collections;

public class IAManagerScript : MonoBehaviour
{


    [SerializeField]
    private int maxIteration;

    [SerializeField]
    private Transform transformIA;

    [SerializeField]
    private Transform transformEnd;

    [SerializeField]
    private Renderer rendererArene;

    private bool bCollision;
    private float prevX;
    private float prevY;

    Vector2[] Direction = new Vector2[8]
    {
        new Vector2(0.0f, 1.0f),
        new Vector2(0.71f, 0.71f),
        new Vector2(1.0f, 0.0f),
        new Vector2(-0.71f, 0.71f),
        new Vector2(-1.0f, 0.0f),
        new Vector2(-0.71f, -0.71f),
        new Vector2(0.0f, -1.0f),
        new Vector2(0.71f, -0.71f)
    };

    float[] curWeight = new float[8];
    MatrixNode[] lowestNodes = new MatrixNode[8];

    [SerializeField]
    private Transform transformPlayer;

    [SerializeField]
    private Renderer rendererPlayer;

    [SerializeField]
    private float speedIA = 1f;

    [SerializeField]
    private Renderer planeRenderer;

    GameManagerScript gameManagerScript;

    public void setGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    public Transform getIaTransform()
    {
        return transformPlayer;
    }

    public Vector2 getNextIaDirection(GameState gameState)
    {
        var node = new MatrixNode();

        var bestNode = AStarFunc(gameState, 0, 0, node);

        return bestNode.direction;
    }

    public class MatrixNode
    {
        public Vector2 direction { get; set; }
        public float weight { get; set; }
    }

    float minWeight;
    int minIndex;

    public MatrixNode AStarFunc(GameState gameState, float prevWeight, int n, MatrixNode node)
    {
        minWeight = float.MaxValue;
        minIndex = -1;

        for (var i = 0; i < 8; i++)
        {
            gameState.iaDirection = Direction[i];

            ///////////////////////////////////////////////////////////////////////////////////////
            //                            DECLARATION DU POIDS                                   //
            ///////////////////////////////////////////////////////////////////////////////////////
            curWeight[i] = 1 / gameManagerScript.CollisionManagerScript.GetGameStateWeight(gameState);

            if (curWeight[i] == -1)
            {
                curWeight[i] = float.MaxValue;
            }
            else
            {
                curWeight[i] += prevWeight;
            }

            node.direction = Direction[i];
            node.weight = curWeight[i];

            if (++n < maxIteration)
            {
                lowestNodes[i] = AStarFunc(gameState, curWeight[i], n, node);
            }
            else
            {
                lowestNodes[i] = node;
            }
        }

        for (var i = 0; i < 8; i++)
        {
            if (lowestNodes[i].weight < minWeight)
            {
                minWeight = lowestNodes[i].weight;
                minIndex = i;
            }
        }

        return lowestNodes[minIndex];
    }

    public void ApplyStateToIa(GameState gameState)
    {
        transformIA.position = new Vector3(gameState.iaPosition.x + gameState.iaDirection.x * speedIA * Time.deltaTime, 0.0f, gameState.iaPosition.z + gameState.iaDirection.y * speedIA * Time.deltaTime);
    }
}
