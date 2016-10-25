using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class IAMoveScriptToEnd : MonoBehaviour
{
    [SerializeField]
    private Transform transformIA;

    [SerializeField]
    private Transform transformEnd;

    [SerializeField]
    private Renderer rendererArene;

    [SerializeField]
    private float speedIA = 1f;

    private Stack<matrixNode> path;
    private bool bCollision;
    private float prevX;
    private float prevY;

    // Use this for initialization
    void Start ()
    {
        bCollision = false;
        path = unitTest_AStar();
    }
	
	// Update is called once per frame
	void Update ()
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
    }

    public class matrixNode
    {
        public float fr = 0, to = 0, sum = 0;
        public float x, y;
        public matrixNode parent;
    }

    public Stack<matrixNode> unitTest_AStar()
    {
        // Ici on aura notre porte d'entrée et notre porte de sortie
        float fromX = transformIA.position.x;
        float fromY = transformIA.position.z;
        float toX = transformEnd.position.x;
        float toY = transformEnd.position.z;

        // On récupère ici le dernier maillon de la chaine
        matrixNode endNode = AStarFunc(fromX, fromY, toX, toY);
        
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
    }

    public matrixNode AStarFunc(float fromX, float fromY, float toX, float toY)
    {
        Dictionary<string, matrixNode> greens = new Dictionary<string, matrixNode>(); 
        // Dictionnaire des positions déjà occupée, pour éviter la marche arrière
        Dictionary<string, matrixNode> reds = new Dictionary<string, matrixNode>(); 

        matrixNode startNode = new matrixNode { x = fromX, y = fromY };
        string key = startNode.x.ToString() + startNode.y.ToString();
        greens.Add(key, startNode);

        Func<KeyValuePair<string, matrixNode>> smallestGreen = () =>
        {
            KeyValuePair<string, matrixNode> smallest = greens.ElementAt(0);

            foreach (KeyValuePair<string, matrixNode> item in greens)
            {
                if (item.Value.sum < smallest.Value.sum)
                    smallest = item;
                else if (item.Value.sum == smallest.Value.sum
                        && item.Value.to < smallest.Value.to)
                    smallest = item;
            }

            return smallest;
        };


        //On a la liste des voisins 
        List<KeyValuePair<int, int>> neighbors = new List<KeyValuePair<int, int>>()
                                        { new KeyValuePair<int, int>(-1,0),
                                            new KeyValuePair<int, int>(-1, 1),
                                            new KeyValuePair<int, int>(0,1),
                                            new KeyValuePair<int, int>(1, 1),
                                            new KeyValuePair<int, int>(1, 0),
                                            new KeyValuePair<int, int>(1, -1),
                                            new KeyValuePair<int, int>(0,-1),
                                            new KeyValuePair<int, int>(-1, -1)};


        // On récupère les limites du terrain
        float maxX = rendererArene.bounds.size.z / 2; 
        float minX = -maxX;
        if (maxX == 0)
            return null;

        float maxY = rendererArene.bounds.size.x / 2;
        float minY = -maxY;

        while (true)
        {
            if (greens.Count == 0)
                return null;

            KeyValuePair<string, matrixNode> current = smallestGreen();

            // Arrivée à la sortie, on sort de la boucle
            if (current.Value.x == toX && current.Value.y == toY)
                return current.Value;

            greens.Remove(current.Key);
            reds.Add(current.Key, current.Value);

            foreach (KeyValuePair<int, int> plusXY in neighbors)
            {
                float nbrX = current.Value.x + plusXY.Key;
                float nbrY = current.Value.y + plusXY.Value;
                string nbrKey = nbrX.ToString() + nbrY.ToString();

                if (nbrX < minX || nbrY < minY || nbrX >= maxX || nbrY >= maxY
                    || bCollision 
                    || reds.ContainsKey(nbrKey))
                    continue;

                if (greens.ContainsKey(nbrKey))
                {
                    matrixNode curNbr = greens[nbrKey];
                    float from = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                    if (from < curNbr.fr)
                    {
                        curNbr.fr = from;
                        curNbr.sum = curNbr.fr + curNbr.to;
                        curNbr.parent = current.Value;
                    }
                }
                else
                {
                    matrixNode curNbr = new matrixNode { x = nbrX, y = nbrY };
                    curNbr.fr = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                    curNbr.to = Math.Abs(nbrX - toX) + Math.Abs(nbrY - toY);
                    curNbr.sum = curNbr.fr + curNbr.to;
                    curNbr.parent = current.Value;
                    greens.Add(nbrKey, curNbr);
                }
            }
        }
    }

    void moveIA(int i)
    {
        switch (i)
        {
            case 1://up
                //transformIA.position = new Vector3(transformIA.position.x, transformIA.position.y, transformIA.position.z + speedIA);
                transformIA.Translate(Vector3.forward * Time.deltaTime * speedIA);
                Debug.Log("Monte");
                break;
            case 2://up right
                transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z + speedIA);
                //transformIA.Translate(Vector3.forward * Time.deltaTime);
                Debug.Log("Monte + Droite");
                break;
            case 3://right
                //transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z);
                transformIA.Translate(Vector3.right * Time.deltaTime * speedIA);
                Debug.Log("Droite");
                break;
            case 4://down right
                transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z - speedIA);
                //transformIA.Translate(Vector3.forward * Time.deltaTime);
                Debug.Log("Descend + Droite");
                break;
            case 5://down
                //transformIA.position = new Vector3(transformIA.position.x, transformIA.position.y, transformIA.position.z - speedIA);
                transformIA.Translate(Vector3.back * Time.deltaTime * speedIA);
                Debug.Log("Descend");
                break;
            case 6://down left
                transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y, transformIA.position.z - speedIA);
                //transformIA.Translate(Vector3.back * Time.deltaTime);
                Debug.Log("Descend + Gauche");
                break;
            case 7://left
                //transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y, transformIA.position.z);
                transformIA.Translate(Vector3.left * Time.deltaTime * speedIA);
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
