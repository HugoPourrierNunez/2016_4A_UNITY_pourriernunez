using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CollisionManagerScript : MonoBehaviour
{

    float bottomWall;
    float topWall;
    float rightWall;
    float leftWall;
    float sizeDivise;

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

    [SerializeField]
    float physicSpeed=4;

    [SerializeField]
    float physicSpeedForPrediction = 10;

    int nbBombs;

    [SerializeField]
    float bombRadius = 0.5f;

    float minDistToIA;

    TriangularMatrixScript<float> bombsDistance;


    void Start()
    {
        bottomWall = -planeRenderer.bounds.size.z / 2;
        topWall = planeRenderer.bounds.size.z / 2;
        rightWall = planeRenderer.bounds.size.x / 2;
        leftWall = -planeRenderer.bounds.size.x / 2;
        sizeDivise = rendererPlayer.bounds.size.x / 2;
    }

    public void InitializeBombInfo(GameState gs)
    {
        //Debug.Log("init bomb info");
        nbBombs = bombManagers.Length;
        
        bombsDistance = new TriangularMatrixScript<float>(nbBombs, nbBombs);

        for (var i = 0; i < nbBombs; i++)
        {
            bombManagers[i].initializeBomb(gameManagerScript.MapManagerScript.getPlaneTransform());
            gs.bombs[i].position.x = bombManagers[i].x0;
            gs.bombs[i].position.z = bombManagers[i].z0;
            gs.bombs[i].direction.x = bombManagers[i].dx;
            gs.bombs[i].direction.y = bombManagers[i].dz;
            gs.bombs[i].delay = -1;
            gs.bombs[i].state = BombState.Normal;
        }

        for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance.Set(i, j, Mathf.Sqrt(Mathf.Pow((gs.bombs[i].position.x - gs.bombs[j].position.x), 2)
                                        + Mathf.Pow((gs.bombs[i].position.z - gs.bombs[j].position.z), 2)));
            }
        }
    }

    public float GetGameStateWeight(GameState gameState)
    {
        return HandleBombCollision(gameState).minDistToIA;
    }

    public GameState HandleBombCollision(GameState gameState)
    {
        var nbBombs = gameState.bombs.Length;

        gameState.minDistToIA = 100.0f;

        gameState.iaPosition.x += physicSpeed * Time.deltaTime * gameState.iaDirection.x;
        gameState.iaPosition.z += physicSpeed * Time.deltaTime * gameState.iaDirection.y;

        // MAJ delay bombes 
        for(var y = 0;  y < nbBombs; y++)
        {
            gameState.bombs[y].delay -= (Time.time*1000) - gameState.timeSinceStart;

            if(gameState.bombs[y].delay <= 0)
            {
                gameState.bombs[y].state = BombState.Normal;
            }
        }

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
            /*for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance.Set(i, j, Mathf.Sqrt(Mathf.Pow((gameState.bombs[i].position.x - gameState.bombs[j].position.x), 2)
                                        + Mathf.Pow((gameState.bombs[i].position.z - gameState.bombs[j].position.z), 2)));

                if (bombsDistance.Get(i, j) < 1)
                {
                    BombCollideWithBomb(i, j, gameState.bombs);
                }
            }*/

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

    public void FillNextGameState(GameState actualGameState, GameState nextGameState)
    {
        //actualGameState.Copy(nextGameState);

        var nbBombs = nextGameState.bombs.Length;

        nextGameState.minDistToIA = float.MaxValue;

        /*nextGameState.iaPosition.x += 4 * Time.deltaTime * nextGameState.iaDirection.x;
        nextGameState.iaPosition.z += 4 * Time.deltaTime * nextGameState.iaDirection.y;*/

        // MAJ delay bombes 
        for (var i = 0; i < nbBombs; i++)
        {
            nextGameState.bombs[i].state = actualGameState.bombs[i].state;
            nextGameState.bombs[i].delay = actualGameState.bombs[i].delay - ((Time.time * 1000) - nextGameState.timeSinceStart);

            if (nextGameState.bombs[i].delay <= 0)
            {
                nextGameState.bombs[i].state = BombState.Normal;
            }

            nextGameState.bombs[i].direction = actualGameState.bombs[i].direction;

            nextGameState.bombs[i].position.x = actualGameState.bombs[i].position.x +(physicSpeed * Time.deltaTime * nextGameState.bombs[i].direction.x);
            nextGameState.bombs[i].position.z = actualGameState.bombs[i].position.z + (physicSpeed * Time.deltaTime * nextGameState.bombs[i].direction.y);

            nextGameState.iaPosition.x = actualGameState.iaPosition.x;
            nextGameState.iaPosition.z = actualGameState.iaPosition.z;
            nextGameState.minDistToIA = actualGameState.minDistToIA;

            var distance = Mathf.Sqrt(Mathf.Pow((nextGameState.bombs[i].position.x - nextGameState.iaPosition.x), 2)
                                        + Mathf.Pow((nextGameState.bombs[i].position.z - nextGameState.iaPosition.z), 2));

            if (distance < nextGameState.minDistToIA)
            {
                nextGameState.minDistToIA = distance;
            }

            if (nextGameState.bombs[i].position.x < bottomWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.bottom, nextGameState.bombs);
            }
            else if (nextGameState.bombs[i].position.x > topWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.top, nextGameState.bombs);
            }
            else if (nextGameState.bombs[i].position.z > rightWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.right, nextGameState.bombs);
            }
            else if (nextGameState.bombs[i].position.z < leftWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.left, nextGameState.bombs);
            }
        }

        /*for(var i=0; i<nbBombs;i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                var distance = Mathf.Sqrt(Mathf.Pow((nextGameState.bombs[i].position.x - nextGameState.bombs[j].position.x), 2)
                                        + Mathf.Pow((nextGameState.bombs[i].position.z - nextGameState.bombs[j].position.z), 2));

                if (distance < bombRadius * 2)
                {
                    BombCollideWithBomb(i, j, nextGameState.bombs);
                }
            }
        }*/
        
        nextGameState.score = 1 / nextGameState.minDistToIA 
            + Mathf.Abs(gameManagerScript.MapManagerScript.getGoalTransform().position.x - nextGameState.iaPosition.x) 
            + Mathf.Abs(gameManagerScript.MapManagerScript.getGoalTransform().position.z - nextGameState.iaPosition.z);
        //Debug.Log("Score = " + nextGameState.score);
    }

    public void ApplyState(GameState gameState)
    {
        var length = bombManagers.Length;
        for(var i = 0; i<length;i++)
        {
            bombManagers[i].ApplyBombInfo(gameState.bombs[i]);
        }

        transformPlayer.position += Time.deltaTime * physicSpeed * gameState.iaDirection;
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
    }
}
