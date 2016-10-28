using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LongTermScript : MonoBehaviour
{
    [SerializeField]
    AStarGridScript aStarGridScript;

    [SerializeField]
    int stepBetweenCheckPoints;

    public Vector3[] FindAICheckPoints(Vector3 iaPosition, Vector3 targetPosition)
    {
        Debug.Log("finding CP");
        var path = FindPath(iaPosition, targetPosition);
        var checkPointsCount = (int)Mathf.Ceil(path.Count / stepBetweenCheckPoints);
        var checkPoints = new Vector3[checkPointsCount];
        var index = 0;

        for (var i = 0; i < checkPointsCount; i++)
        {

            Debug.Log("allo?");
            index = ((i + 1) * stepBetweenCheckPoints > path.Count) ? path.Count - 1 : (i + 1) * stepBetweenCheckPoints;
            checkPoints[i] = path[index].worldPosition;
        }
        return checkPoints;
    }

    public List<LongTermNode> FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        var startNode = aStarGridScript.NodeFromWorldPoint(startPosition);
        var targetNode = aStarGridScript.NodeFromWorldPoint(targetPosition);

        List<LongTermNode> neighbours;
        List<LongTermNode> openSet = aStarGridScript.GetNeighbours(startNode);
        var openCount = openSet.Count;

        var node = startNode;
        var index = 0;

        node.closed = true;

        while (openCount > 0)
        {
            node = openSet[0];

            for (var i = 1; i < openCount; i++)
            {
                if (openSet[i].Score < node.Score || (openSet[i].Score == node.Score && openSet[i].cost < node.cost))
                {
                    node = openSet[i];
                    index = i;
                }
            }

            node.closed = true;
            openCount--;

            if (node == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            neighbours = aStarGridScript.GetNeighbours(node);

            for(var i = 0; i < neighbours.Count; i++)
            {
                if (!neighbours[i].walkable || neighbours[i].closed)
                {
                    continue;
                }

                var newCostToNeighbour = node.cost + 1 + neighbours[i].weight;
                if (newCostToNeighbour < neighbours[i].Score || !neighbours[i].visited)
                {
                    neighbours[i].cost = node.cost + 1;
                    neighbours[i].parent = node;

                    if (!neighbours[i].visited)
                    {
                        neighbours[i].visited = true;
                        openSet.Add(neighbours[i]);
                        openCount++;
                    }
                }
            }
            openSet.RemoveAt(index);
        }

        return null;
    }

    List<LongTermNode> RetracePath(LongTermNode startNode, LongTermNode endNode)
    {
        var path = new List<LongTermNode>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }
}
