using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarGridScript : MonoBehaviour
{
    GameManagerScript gameManagerScript;

    public void SetGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    public LayerMask unwalkableMask;
    public Vector3 gridWorldSize;
    public float nodeRadius;

    LongTermNode[] grid;

    [SerializeField]
    Transform[] obstacles;

    [SerializeField]
    float obstacleRadius;

    float nodeDiameter;
    int gridSizeX;
    int gridSizeZ;

    void InitializeeGridDimensions()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = (int)(gameManagerScript.MapManagerScript.GetPlaneTransform().lossyScale.x / nodeDiameter);
        //gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeZ = (int)(gameManagerScript.MapManagerScript.GetPlaneTransform().lossyScale.z / nodeDiameter);
        //gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
        CreateGrid();
    }

    public LongTermNode[] GetGrid()
    {
        return grid;
    }

    void CreateGrid()
    {
        grid = new LongTermNode[gridSizeX * gridSizeZ];
        var worldBottomLeft = new Vector3(gameManagerScript.MapManagerScript.GetPlaneTransform().position.x - gameManagerScript.MapManagerScript.GetPlaneTransform().lossyScale.x / 2, 
            0.0f,
            gameManagerScript.MapManagerScript.GetPlaneTransform().position.z - gameManagerScript.MapManagerScript.GetPlaneTransform().lossyScale.z / 2);

        var walkable = true;
        var casePosition = new Vector3(0.0f, 0.0f, 0.0f);

        for (var x = 0; x < gridSizeX; x++)
        {
            for (var z = 0; z < gridSizeZ; z++)
            {
                casePosition.x = x;
                casePosition.z = z;
                for(var i = 0; i < obstacles.Length; i++)
                {
                    if(obstacles[i].position.x + obstacleRadius > x
                        && obstacles[i].position.x - obstacleRadius < x
                        && obstacles[i].position.z + obstacleRadius > z
                        && obstacles[i].position.z - obstacleRadius < z)
                    {
                        walkable = false;
                    }
                    grid[x * gridSizeX + z ] = new LongTermNode(walkable, worldBottomLeft + casePosition, x, z);
                    grid[x * gridSizeX + z].weight = gridSizeX + gridSizeZ - x - z;
                }
            }
        }
    }

    public List<LongTermNode> GetNeighbours(LongTermNode node)
    {
        var neighbours = new List<LongTermNode>();
        var i = 0;

        for (var z = -1; z <= 1; z++)
        {
            for (var x = -1; x <= 1; x++)
            {
                if (x == 0 && z == 0)
                {
                    continue;
                }

                var checkX = node.gridX + x;
                var checkZ = node.gridZ + z;

                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                {
                    neighbours.Add(grid[checkX * gridSizeX + checkZ]);
                    i++;
                }
            }
        }

        return neighbours;
    }

    public LongTermNode[] GetOpenNodes(int openCount)
    {
        var openNodes = new LongTermNode[openCount];
        var i = 0;

        for(var x = 0; x < gridSizeX; x++)
        {
            for (var z = 0; z < gridSizeZ; z++)
            {
                if(!grid[x * gridSizeX + z].closed)
                {
                    openNodes[i] = grid[x * gridSizeX + z];
                    i++;
                }
            }
        }

        return openNodes;
    }


    public LongTermNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        var percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        var percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        var x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        var z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
        return grid[x * gridSizeX + z];
    }

    public List<LongTermNode> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (LongTermNode n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
