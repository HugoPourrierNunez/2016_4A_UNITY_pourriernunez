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

    [SerializeField]
    float iaSpeed;

    [SerializeField]
    float bombSpeed;

    int nbBombs;

    [SerializeField]
    float bombRadius = 0.5f;

    float iaRadius;
    float minDistToIA;

    TriangularMatrixScript<float> bombsDistance;


    void Start()
    {
        bottomWall = -planeRenderer.bounds.size.z / 2;
        topWall = planeRenderer.bounds.size.z / 2;
        rightWall = planeRenderer.bounds.size.x / 2;
        leftWall = -planeRenderer.bounds.size.x / 2;
        iaRadius = rendererPlayer.bounds.size.x / 2;
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
            gs.bombs[i].direction.z = bombManagers[i].dz;
            gs.bombs[i].delay = -1;
            gs.bombs[i].state = BombState.Normal;
        }

        /*for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance.Set(i, j, Mathf.Sqrt(Mathf.Pow((gs.bombs[i].position.x - gs.bombs[j].position.x), 2)
                                        + Mathf.Pow((gs.bombs[i].position.z - gs.bombs[j].position.z), 2)));
            }
        }*/
    }

    public float GetGameStateWeight(GameState gameState)
    {
        HandleBombCollision(gameState);
        return gameState.minDistToIA;
    }

    public void HandleBombCollision(GameState gameState)
    {
        var nbBombs = gameState.bombs.Length;
        var minIndex = -1;
        var minXZ = float.MaxValue;

        gameState.minDistToIA = 100.0f;

        gameState.iaPosition.x += iaSpeed * Time.deltaTime * gameState.iaDirection.x;
        gameState.iaPosition.z += iaSpeed * Time.deltaTime * gameState.iaDirection.z;

        Transform goalTransform = gameManagerScript.MapManagerScript.getGoalTransform();

        if (transformPlayer.position.x > goalTransform.position.x-goalTransform.localScale.x/2 
            && transformPlayer.position.x < goalTransform.position.x + goalTransform.localScale.x / 2
            && transformPlayer.position.z > goalTransform.position.z - goalTransform.localScale.z / 2
            && transformPlayer.position.z < goalTransform.position.z + goalTransform.localScale.z / 2)
        {
            gameManagerScript.StateManagerScript.EndGame(false);
            return;
        }

        for (var i = 0; i < nbBombs; i++)
        {
            gameState.bombs[i].delay -= (Time.time * 1000) - gameState.timeSinceStart;

            if (gameState.bombs[i].delay <= 0)
            {
                gameState.bombs[i].state = BombState.Normal;
            }

            gameState.bombs[i].position.x += bombSpeed * Time.deltaTime * gameState.bombs[i].direction.x;
            gameState.bombs[i].position.z += bombSpeed * Time.deltaTime * gameState.bombs[i].direction.z;

            var XZ = Mathf.Abs(gameState.bombs[i].position.x - gameState.iaPosition.x) + Mathf.Abs(gameState.bombs[i].position.z - gameState.iaPosition.z);

            if (XZ < minXZ)
            {
                minIndex = i;
                minXZ = XZ;
            }
        }

        gameState.minDistToIA = Mathf.Sqrt(Mathf.Pow((gameState.bombs[minIndex].position.x - gameState.iaPosition.x), 2)
                                                    + Mathf.Pow((gameState.bombs[minIndex].position.z - gameState.iaPosition.z), 2));

        if (gameState.minDistToIA <= iaRadius + bombRadius)
        {
            gameManagerScript.StateManagerScript.EndGame(true);
            return;
        }

        for (var i = 0; i < nbBombs; i++)
        {
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
        return;
    }

    public void FillNextGameState(GameState actualGameState, GameState nextGameState, Vector3 direction)
    {
        var nbBombs = nextGameState.bombs.Length;
        var minIndex = -1;
        var minXZ = float.MaxValue;

        nextGameState.minDistToIA = float.MaxValue;

        nextGameState.iaPosition = actualGameState.iaPosition + direction * Time.deltaTime * iaSpeed;

        if(nextGameState.iaPosition.x + iaRadius > rightWall 
            || nextGameState.iaPosition.x - iaRadius < leftWall
            || nextGameState.iaPosition.z + iaRadius > topWall
            || nextGameState.iaPosition.z - iaRadius < bottomWall)
        {
            return;
        }

        for (var i = 0; i < nbBombs; i++)
        {
            nextGameState.bombs[i].state = actualGameState.bombs[i].state;
            nextGameState.bombs[i].delay = actualGameState.bombs[i].delay - ((Time.time * 1000) - nextGameState.timeSinceStart);

            if (nextGameState.bombs[i].delay <= 0)
            {
                nextGameState.bombs[i].state = BombState.Normal;
            }

            nextGameState.bombs[i].direction = actualGameState.bombs[i].direction;

            nextGameState.bombs[i].position.x = actualGameState.bombs[i].position.x +(bombSpeed * Time.deltaTime * nextGameState.bombs[i].direction.x);
            nextGameState.bombs[i].position.z = actualGameState.bombs[i].position.z + (bombSpeed * Time.deltaTime * nextGameState.bombs[i].direction.z);

            nextGameState.iaPosition.x = actualGameState.iaPosition.x;
            nextGameState.iaPosition.z = actualGameState.iaPosition.z;
            nextGameState.minDistToIA = actualGameState.minDistToIA;

            var XZ = Mathf.Abs(nextGameState.bombs[i].position.x - nextGameState.iaPosition.x) + Mathf.Abs(nextGameState.bombs[i].position.z - nextGameState.iaPosition.z);

            if (XZ < minXZ)
            {
                minIndex = i;
                minXZ = XZ;
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

        nextGameState.minDistToIA = Mathf.Sqrt(Mathf.Pow((nextGameState.bombs[minIndex].position.x - nextGameState.iaPosition.x), 2)
                                                    + Mathf.Pow((nextGameState.bombs[minIndex].position.z - nextGameState.iaPosition.z), 2));

        if (nextGameState.minDistToIA <= iaRadius + bombRadius)
        {
            return;
        }
    }

    public void ApplyState(GameState gameState)
    {
        var length = bombManagers.Length;
        for(var i = 0; i<length;i++)
        {
            bombManagers[i].ApplyBombInfo(gameState.bombs[i]);
        }

        transformPlayer.position = gameState.iaPosition;
    }

    private void BombCollideWithBomb(int i, int j, BombInfo[] bombs)
    {
        var tmpDir = bombs[i].direction;
        bombs[i].direction = bombs[j].direction;
        bombs[j].direction = tmpDir;

        bombManagers[i].dx = bombs[i].direction.x;
        bombManagers[i].dz = bombs[i].direction.z;
        bombManagers[j].dx = bombs[j].direction.x;
        bombManagers[j].dz = bombs[j].direction.z;
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
                bombs[i].direction.z = -Mathf.Abs(bombs[i].direction.z);
                break;
            case Walls.left:
                bombs[i].direction.z = Mathf.Abs(bombs[i].direction.z);
                break;
        }
    }
}
