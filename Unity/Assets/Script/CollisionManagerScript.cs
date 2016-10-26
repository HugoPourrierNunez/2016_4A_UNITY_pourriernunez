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

    [SerializeField]
    private Renderer planeRenderer;

    [SerializeField]
    private Transform transformPlayer;

    [SerializeField]
    private Renderer rendererPlayer;

    int nbBombs;
    float bombRadius = 0.5f;
    BombInfo[] bombs;
    float minDistToIA;

    TriangularMatrixScript<float> bombsDistance;

    public BombInfo[] InitializeBombInfo()
    {
        //Debug.Log("init bomb info");
        nbBombs = bombManagers.Length;

        bombs = new BombInfo[nbBombs];
        bombsDistance = new TriangularMatrixScript<float>(nbBombs, nbBombs);

        for (var i = 0; i < nbBombs; i++)
        {
            bombManagers[i].initializeBomb(gameManagerScript.MapManagerScript.getPlaneTransform());
            bombs[i].position.x = bombManagers[i].x0;
            bombs[i].position.z = bombManagers[i].z0;
            bombs[i].direction.x = bombManagers[i].dx;
            bombs[i].direction.y = bombManagers[i].dz;
            bombs[i].delay = -1;
            bombs[i].state = BombState.Normal;
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

    public float GetGameStateWeight(GameState gameState)
    {
        gameState = HandleBombCollision(gameState);

        return gameState.minDistToIA;
    }

    public GameState HandleBombCollision(GameState gameState)
    {
        var nbBombs = bombs.Length;

        var bottomWall = -planeRenderer.bounds.size.z / 2;
        var topWall = planeRenderer.bounds.size.z / 2;
        var rightWall = planeRenderer.bounds.size.x / 2;
        var leftWall = -planeRenderer.bounds.size.x / 2;
        var sizeDivise = rendererPlayer.bounds.size.x / 2;

        gameState.minDistToIA = 100.0f;

        gameState.iaPosition.x += 4 * Time.deltaTime * gameState.iaDirection.x;
        gameState.iaPosition.z += 4 * Time.deltaTime * gameState.iaDirection.y;

        //Modif
        Transform goalTransform = gameManagerScript.MapManagerScript.getGoalTransform();

        if (transformPlayer.position.x > goalTransform.position.x-goalTransform.localScale.x/2 
            && transformPlayer.position.x < goalTransform.position.x + goalTransform.localScale.x / 2
            && transformPlayer.position.z > goalTransform.position.z - goalTransform.localScale.z / 2
            && transformPlayer.position.z < goalTransform.position.z + goalTransform.localScale.z / 2)
        {
            gameManagerScript.StateManagerScript.EndGame(false);
            return gameState;
        }

        //

        for (var i = 0; i < nbBombs; i++)
        {
            gameState.bombs[i].position.x += 4 * Time.deltaTime * gameState.bombs[i].direction.x;
            gameState.bombs[i].position.z += 4 * Time.deltaTime * gameState.bombs[i].direction.y;

            if(Mathf.Sqrt(Mathf.Pow((gameState.bombs[i].position.x - transformPlayer.position.x), 2)
                                        + Mathf.Pow((gameState.bombs[i].position.z - transformPlayer.position.z), 2)) < gameState.minDistToIA)
            {
                gameState.minDistToIA = Mathf.Sqrt(Mathf.Pow((gameState.bombs[i].position.x - transformPlayer.position.x), 2)
                                            + Mathf.Pow((gameState.bombs[i].position.z - transformPlayer.position.z), 2));
            }

            if(gameState.minDistToIA <= sizeDivise + bombRadius)
            {
                gameManagerScript.StateManagerScript.EndGame(true);
                gameState.minDistToIA = -1;
                return gameState;
            }
        }

        for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance.Set(i, j, Mathf.Sqrt(Mathf.Pow((gameState.bombs[i].position.x - gameState.bombs[j].position.x), 2)
                                        + Mathf.Pow((gameState.bombs[i].position.z - gameState.bombs[j].position.z), 2)));

                if (bombsDistance.Get(i, j) < 1)
                {
                    BombCollideWithBomb(i, j, gameState.bombs);
                }
            }

            if (gameState.bombs[i].position.x < bottomWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.bottom, gameState.bombs);
            }

            if (gameState.bombs[i].position.x > topWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.top, gameState.bombs);
            }

            if (gameState.bombs[i].position.z > rightWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.right, gameState.bombs);
            }

            if (gameState.bombs[i].position.z < leftWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.left, gameState.bombs);
            }
        }
        return gameState;
    }

    public void applyStateToBombs(GameState gameState)
    {
        var length = bombManagers.Length;
        for(var i = 0; i<length;i++)
        {
            bombManagers[i].ApplyBombInfo(gameState.bombs[i]);
        }
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
