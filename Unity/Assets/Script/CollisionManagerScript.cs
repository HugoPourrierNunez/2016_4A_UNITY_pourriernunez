using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CollisionManagerScript : MonoBehaviour
{

    /*private struct Bomb
    {
        public float x0;
        public float z0;
        public float dx;
        public float dz;
        public float t;
    }*/

    private enum Walls
    {
        bottom,
        top,
        right,
        left
    }

    public BombManagerScript[] getBombManagers()
    {
        return bombManagers;
    }

    [SerializeField]
    public BombManagerScript[] bombManagers;

    private int nbBombs;
    private float bombRadius = 0.5f;
    private float bottomWall = -20.0f;
    private float topWall = 20.0f;
    private float rightWall = 20.0f;
    private float leftWall = -20.0f;
    private BombInfo[] bombs;
    private float[,] bombsDistance;

	void OnEnable ()
    {
        nbBombs = bombManagers.Length;

        bombs = new BombInfo[nbBombs];
        bombsDistance = new float[nbBombs , nbBombs];

        for (var i = 0; i < nbBombs; i++)
        {
            bombs[i].position.x = bombManagers[i].x0;
            bombs[i].position.z = bombManagers[i].z0;
            bombs[i].direction.x = bombManagers[i].dx;
            bombs[i].direction.y = bombManagers[i].dz;
        }

        for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance[i,j] = Mathf.Sqrt(Mathf.Pow((bombs[i].position.x - bombs[j].position.x), 2) 
                                        + Mathf.Pow((bombs[i].position.z - bombs[j].position.z), 2));
            }
        }
    }

    void Update()
    {
        for (var i = 0; i < nbBombs; i++)
        {
            bombs[i].position.x += 4 * Time.deltaTime * bombs[i].direction.x;
            bombs[i].position.z += 4 * Time.deltaTime * bombs[i].direction.y;
        }

        for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance[i,j] = Mathf.Sqrt(Mathf.Pow((bombs[i].position.x - bombs[j].position.x), 2)
                                        + Mathf.Pow((bombs[i].position.z - bombs[j].position.z), 2));
                
                if (bombsDistance[i,j] < 1)
                {
                    BombCollideWithBomb(i, j);
                }
            }

            if (bombs[i].position.x < bottomWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.bottom);
            }

            else if (bombs[i].position.x > topWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.top);
            }

            else if (bombs[i].position.z > rightWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.right);
            }

            else if (bombs[i].position.z < leftWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.left);
            }
        }
    }

    void BombCollideWithBomb(int i, int j)
    {
        var tmpDir = bombs[i].direction;
        bombs[i].direction = bombs[j].direction;
        bombs[j].direction = tmpDir;

        bombManagers[i].dx = bombs[i].direction.x;
        bombManagers[i].dz = bombs[i].direction.y;
        bombManagers[j].dx = bombs[j].direction.x;
        bombManagers[j].dz = bombs[j].direction.y;
    }

    void BombCollideWithWall(int i, Walls wall)
    {
        switch (wall)
        {
            case Walls.bottom:
                bombs[i].direction.x = Mathf.Abs(bombs[i].direction.x);
                break;
            case Walls.top:
                bombs[i].direction.x = -Mathf.Abs(bombs[i].direction.x);
                break;
            case Walls.right:
                bombs[i].direction.y = -Mathf.Abs(bombs[i].direction.y);
                break;
            case Walls.left:
                bombs[i].direction.y = Mathf.Abs(bombs[i].direction.y);
                break;
        }

        bombManagers[i].dx = bombs[i].direction.x;
        bombManagers[i].dz = bombs[i].direction.y;
    }
}
