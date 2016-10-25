using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CollisionManagerScript : MonoBehaviour
{
    GameManagerScript gameManagerScript;

    public void setGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

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

    int nbBombs;
    float bombRadius = 0.5f;
    float bottomWall = -20.0f;
    float topWall = 20.0f;
    float rightWall = 20.0f;
    float leftWall = -20.0f;
    BombInfo[] bombs;
    TriangularMatrixScript<float> bombsDistance;

    public BombInfo[] InitializeBombInfo()
    {
        Debug.Log("init bomb info");
        nbBombs = bombManagers.Length;

        bombs = new BombInfo[nbBombs];
        bombsDistance = new TriangularMatrixScript<float>(nbBombs, nbBombs);

        for (var i = 0; i < nbBombs; i++)
        {
            bombs[i].position.x = bombManagers[i].x0;
            bombs[i].position.z = bombManagers[i].z0;
            bombs[i].direction.x = bombManagers[i].dx;
            bombs[i].direction.y = bombManagers[i].dz;
            bombs[i].delay = -1;
            bombs[i].state = BombState.Normal;
            bombManagers[i].initializeBomb();
        }

        for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance.Set(i, j, Mathf.Sqrt(Mathf.Pow((bombs[i].position.x - bombs[j].position.x), 2)
                                        + Mathf.Pow((bombs[i].position.z - bombs[j].position.z), 2)));
            }
        }

        return bombs;
    }

    public BombInfo[] HandleBombCollision(GameState gameState)
    {
        var bombs = gameState.bombs;
        var nbBombs = bombs.Length;

        for (var i = 0; i < nbBombs; i++)
        {
            bombs[i].position.x += 4 * Time.deltaTime * bombs[i].direction.x;
            bombs[i].position.z += 4 * Time.deltaTime * bombs[i].direction.y;
        }

        for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance.Set(i, j, Mathf.Sqrt(Mathf.Pow((bombs[i].position.x - bombs[j].position.x), 2)
                                        + Mathf.Pow((bombs[i].position.z - bombs[j].position.z), 2)));

                if (bombsDistance.Get(i, j) < 1)
                {
                    BombCollideWithBomb(i, j, bombs);
                }
            }

            if (bombs[i].position.x < bottomWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.bottom, bombs);
            }

            if (bombs[i].position.x > topWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.top, bombs);
            }

            if (bombs[i].position.z > rightWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.right, bombs);
            }

            if (bombs[i].position.z < leftWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.left, bombs);
            }
        }

        return bombs;
    }

    private void BombCollideWithBomb(int i, int j, BombInfo[] bombs)
    {
        var tmpDir = bombs[i].direction;
        bombs[i].direction = bombs[j].direction;
        bombs[j].direction = tmpDir;

        bombManagers[i].dx = bombs[i].direction.x;
        bombManagers[i].dz = bombs[i].direction.y;
        bombManagers[j].dx = bombs[j].direction.x;
        bombManagers[j].dz = bombs[j].direction.y;
    }

    private void BombCollideWithWall(int i, Walls wall, BombInfo[] bombs)
    {
        switch (wall)
        {
            case Walls.bottom:
                bombs[i].direction.x = Mathf.Abs(bombs[i].direction.x);
                bombManagers[i].dx = bombs[i].direction.x;
                break;
            case Walls.top:
                bombs[i].direction.x = -Mathf.Abs(bombs[i].direction.x);
                bombManagers[i].dx = bombs[i].direction.x;
                break;
            case Walls.right:
                bombs[i].direction.y = -Mathf.Abs(bombs[i].direction.y);
                bombManagers[i].dz = bombs[i].direction.y;
                break;
            case Walls.left:
                bombs[i].direction.y = Mathf.Abs(bombs[i].direction.y);
                bombManagers[i].dz = bombs[i].direction.y;
                break;
        }
    }
}
