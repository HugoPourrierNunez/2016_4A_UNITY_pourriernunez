using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class IAMoveScriptToEnd : MonoBehaviour
{
    GameManagerScript gameManagerScript;

    [SerializeField]
    private int maxIteration;

    [SerializeField]
    private Transform transformIA;

    [SerializeField]
    private Transform transformEnd;

    [SerializeField]
    private Renderer rendererArene;

    [SerializeField]
    private float speedIA = 1f;

    private Stack<MatrixNode> path;
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

    public void setGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    // Use this for initialization
    void Start()
    {
        bCollision = false;
        //path = unitTest_AStar();
    }

    public Vector2 getNextIaDirection(GameState gameState)
    {
        var node = new MatrixNode();
        //var wrapper = new GameStateWrapper(gameState);

        var bestNode = AStarFunc(gameState, 0, 0, node);

        return bestNode.direction;
    }

    // Update is called once per frame
    /*void Update ()
    {
        if (path.Count > 0)
        {
            matrixNode node = path.Pop();

            Debug.Log("(" + node.x + "," + node.y + ")");

            if (prevX == 0 && prevY == 0)
            {
                prevX = node.x;
                prevY = node.y;
            }

            if (prevX < node.x && prevY < node.y)
            {
                moveIA(2);

                prevX = node.x;
                prevY = node.y;
            }

            if (prevX > node.x && prevY > node.y)
            {
                moveIA(6);

                prevX = node.x;
                prevY = node.y;
            }

            if (prevX > node.x && prevY < node.y)
            {
                moveIA(8);

                prevX = node.x;
                prevY = node.y;
            }

            if (prevX < node.x && prevY > node.y)
            {
                moveIA(4);

                prevX = node.x;
                prevY = node.y;
            }

            if (prevX == node.x && prevY < node.y)
            {
                moveIA(1);

                prevX = node.x;
                prevY = node.y;
            }

            if (prevX == node.x && prevY > node.y)
            {
                moveIA(5);

                prevX = node.x;
                prevY = node.y;
            }

            if (prevX > node.x && prevY == node.y)
            {
                moveIA(7);

                prevX = node.x;
                prevY = node.y;
            }

            if (prevX < node.x && prevY == node.y)
            {
                moveIA(3);

                prevX = node.x;
                prevY = node.y;
            }
        }
    }*/

    public class MatrixNode
    {
        //public matrixNode parent;
        public Vector2 direction { get; set; }
        public float weight { get; set; }
    }

    /*public class GameStateWrapper
    {
        public BombInfo[] bombs { get; set; }
        public Vector3 iaPosition { get; set; }
        public Vector2 iaDirection { get; set; }
        public float distToIa { get; set; }

        public GameStateWrapper(GameState gameState)
        {
            this.bombs = gameState.bombs;
            this.iaPosition = gameState.iaPosition;
            this.iaDirection = gameState.iaDirection;
            this.distToIa = gameState.minDistToIA;
        }

        public float GetGameStateWeight()
        {

            return 1.0f;
        }
    }

    public Stack<matrixNode> unitTest_AStar()
    {
        // Ici on aura notre porte d'entrée et notre porte de sortie
        float fromX = transformIA.position.x;
        float fromY = transformIA.position.z;
        float toX = transformEnd.position.x;
        float toY = transformEnd.position.z;

        // On récupère ici le dernier maillon de la chaine
        matrixNode endNode = AStarFunc(gameManagerScript.ActualGameState, fromX, fromY, toX, toY);
        
        // On instancie la pile du chemin le plus court
        // On aura donc une pile de GameState ici
        Stack<matrixNode> path = new Stack<matrixNode>();

        // Depuis le dernier maillon, on remonte jusqu'à l'origine de la chaine, tout en ajoutant l'élément en cours à la stack
        while (endNode.x != fromX || endNode.y != fromY)
        {
            path.Push(endNode);
            endNode = endNode.parent;
        }
        path.Push(endNode);

        // Pour les tests on pourra afficher par exemple l'action de la gameState
        Debug.Log("The shortest path from  " +
                            "(" + fromX + "," + fromY + ")  to " +
                            "(" + toX + "," + toY + ")  is:  \n");
        return path;
    }*/

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

            if (++n > maxIteration)
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

    void moveIA(int i)
    {
        switch (i)
        {
            case 1://up
                transformIA.position = new Vector3(transformIA.position.x, transformIA.position.y, transformIA.position.z + speedIA);
                //transformIA.Translate(Vector3.forward * Time.deltaTime * speedIA);
                Debug.Log("Monte");
                break;
            case 2://up right
                transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z + speedIA);
                //transformIA.Translate(Vector3.forward * Time.deltaTime);
                Debug.Log("Monte + Droite");
                break;
            case 3://right
                transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z);
                //transformIA.Translate(Vector3.right * Time.deltaTime * speedIA);
                Debug.Log("Droite");
                break;
            case 4://down right
                transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z - speedIA);
                //transformIA.Translate(Vector3.forward * Time.deltaTime);
                Debug.Log("Descend + Droite");
                break;
            case 5://down
                transformIA.position = new Vector3(transformIA.position.x, transformIA.position.y, transformIA.position.z - speedIA);
                //transformIA.Translate(Vector3.back * Time.deltaTime * speedIA);
                Debug.Log("Descend");
                break;
            case 6://down left
                transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y, transformIA.position.z - speedIA);
                //transformIA.Translate(Vector3.back * Time.deltaTime);
                Debug.Log("Descend + Gauche");
                break;
            case 7://left
                transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y, transformIA.position.z);
                //transformIA.Translate(Vector3.left * Time.deltaTime * speedIA);
                Debug.Log("Gauche");
                break;
            case 8://up left 
                transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y, transformIA.position.z + speedIA);
                //transformIA.Translate(Vector3.back * Time.deltaTime);
                Debug.Log("Monte + Gauche");
                break;
        }
    }
}
