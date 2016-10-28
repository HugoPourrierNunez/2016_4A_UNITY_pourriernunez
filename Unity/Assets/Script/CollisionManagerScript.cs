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

    public void SetGameManagerScript(GameManagerScript gmScript)
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
    float physicSpeed = 4;

    [SerializeField]
    float physicSpeedForPrediction = 10;

    [SerializeField]
    float iaSpeed;

    [SerializeField]
    float bombSpeed;

    [SerializeField]
    float distanceIgnore = 50;


    [SerializeField]
    float coefPrediction = 10;

    int nbBombs;

    [SerializeField]
    float bombRadius = 0.5f;

    float explosionRadius;
    public float iaRadius;
    float minDistToIA;

    void OnEnable()
    {
        bottomWall = - planeRenderer.bounds.size.z / 2;
        topWall = planeRenderer.bounds.size.z / 2;
        rightWall = planeRenderer.bounds.size.x / 2;
        leftWall = - planeRenderer.bounds.size.x / 2;
        iaRadius = rendererPlayer.bounds.size.x / 2;
    }

    public void InitializeBombInfo(GameState gameState)
    {
        nbBombs = gameState.bombs.Length;

        for (var i = 0; i < nbBombs; i++)
        {
            gameState.bombs[i].position.x = UnityEngine.Random.Range(planeRenderer.transform.position.x - planeRenderer.transform.localScale.x * 5 + bombManagers[i].transform.localScale.x / 2, planeRenderer.transform.position.x + planeRenderer.transform.localScale.x * 5 - bombManagers[i].transform.localScale.x / 2); ;
            gameState.bombs[i].position.z = UnityEngine.Random.Range(planeRenderer.transform.position.z - planeRenderer.transform.localScale.z * 5 + bombManagers[i].transform.localScale.z / 2, planeRenderer.transform.position.z + planeRenderer.transform.localScale.z * 5 - bombManagers[i].transform.localScale.z / 2);

            gameState.bombs[i].direction = IAScript.Direction[UnityEngine.Random.Range(0, 8)];
            gameState.bombs[i].delay = -1;
            gameState.bombs[i].state = BombState.Normal;
            gameState.distanceIaToBombs[i] = Mathf.Sqrt(Mathf.Pow(gameState.bombs[i].position.x - gameState.iaPosition.x, 2) + Mathf.Pow(gameState.bombs[i].position.z - gameState.iaPosition.z, 2));

            bombManagers[i].SetScaleLaser(gameManagerScript.MapManagerScript.GetPlaneTransform());
            bombManagers[i].ResetState();
        }
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
        var deltaTime = Time.fixedDeltaTime;

        gameState.minDistToIA = 100.0f;

        gameState.iaPosition.x += iaSpeed * Time.fixedDeltaTime * gameState.iaDirection.x;
        gameState.iaPosition.z += iaSpeed * Time.fixedDeltaTime * gameState.iaDirection.z;

        Transform goalTransform = gameManagerScript.MapManagerScript.GetGoalTransform();

        /*if (transformPlayer.position.x > goalTransform.position.x-goalTransform.localScale.x/2 
            && transformPlayer.position.x < goalTransform.position.x + goalTransform.localScale.x / 2
            && transformPlayer.position.z > goalTransform.position.z - goalTransform.localScale.z / 2
            && transformPlayer.position.z < goalTransform.position.z + goalTransform.localScale.z / 2)
        {
            gameManagerScript.StateManagerScript.EndGame(false);
            return;
        }*/

        for (var i = 0; i < nbBombs; i++)
        {
            if(gameState.bombs[i].state!= BombState.Normal)
            {
                gameState.bombs[i].delay -= deltaTime*1000;
            }

            //Debug.Log(gameState.bombs[i].direction);
            gameState.bombs[i].position.x += bombSpeed * Time.fixedDeltaTime * gameState.bombs[i].direction.x;
            gameState.bombs[i].position.z += bombSpeed * Time.fixedDeltaTime * gameState.bombs[i].direction.z;

            if (gameState.bombs[i].state == BombState.BOOM)
            {
                explosionRadius = 0.25f;
            }
            else
            {
                explosionRadius = 0;
            }

            var XZ = Mathf.Abs(gameState.bombs[i].position.x - gameState.iaPosition.x) + Mathf.Abs(gameState.bombs[i].position.z - gameState.iaPosition.z) - explosionRadius;

            if (XZ < minXZ)
            {
                minIndex = i;
                minXZ = XZ;
            }
        }

        if (gameState.bombs.Length > 0)
        {
            gameState.minDistToIA = Mathf.Sqrt(Mathf.Pow((gameState.bombs[minIndex].position.x - gameState.iaPosition.x), 2)
                                                        + Mathf.Pow((gameState.bombs[minIndex].position.z - gameState.iaPosition.z), 2));
        }
        else
        {
            minDistToIA = float.MaxValue;
        }


        /*if (gameState.minDistToIA <= iaRadius + bombRadius + (gameState.bombs[minIndex].state == BombState.BOOM ? 1 : 0))
        {
            gameManagerScript.StateManagerScript.EndGame(true);
            return;
        }*/



        for (var i = 0; i < nbBombs; i++)
        {
            BombCollideWithWall(i, gameState);
        }
        return;
    }

    public bool CollisionObstacle(GameState nextGameState)
    {

        return false;
    }

    public void FillNextGameState(GameState actualGameState, GameState nextGameState, Vector3 direction)
    {
        if (CollisionObstacle(nextGameState))
        {
            nextGameState.minDistToIA = 0;
            return;
        }
           

        var nbBombs = nextGameState.bombs.Length;
        var minIndex = -1;
        var minXZ = float.MaxValue;
        float minDiffX, minDiffZ;
        var deltaTime = Time.fixedDeltaTime*coefPrediction;
        var iaDeltaSpeed = iaSpeed* deltaTime;
        var bombDeltaSpeed = iaSpeed* deltaTime;

        nextGameState.minDistToIA = float.MaxValue;

        nextGameState.iaPosition = actualGameState.iaPosition + direction * iaDeltaSpeed;

        if (nextGameState.iaPosition.x + iaRadius > rightWall
            || nextGameState.iaPosition.x - iaRadius < leftWall
            || nextGameState.iaPosition.z + iaRadius > topWall
            || nextGameState.iaPosition.z - iaRadius < bottomWall)
        {
            nextGameState.minDistToIA = 0;
            return;
        }

        float diffX = 0, diffZ = 0, XZ;

        for (var i = 0; i < nbBombs; i++)
        {
            if (nextGameState.bombs[i].state != BombState.Normal)
            {
                nextGameState.bombs[i].delay -= deltaTime * 1000;
            }

            nextGameState.bombs[i].direction = actualGameState.bombs[i].direction;

            nextGameState.bombs[i].position.x = actualGameState.bombs[i].position.x + (bombDeltaSpeed * nextGameState.bombs[i].direction.x);
            nextGameState.bombs[i].position.z = actualGameState.bombs[i].position.z + (bombDeltaSpeed * nextGameState.bombs[i].direction.z);

            if (nextGameState.bombs[i].position.x > nextGameState.iaPosition.x)
                diffX = nextGameState.bombs[i].position.x - nextGameState.iaPosition.x;
            else
                diffX = nextGameState.iaPosition.x - nextGameState.bombs[i].position.x;

            if (nextGameState.bombs[i].position.z > nextGameState.iaPosition.z)
                diffZ = nextGameState.bombs[i].position.z - nextGameState.iaPosition.z;
            else
                diffZ = nextGameState.iaPosition.z - nextGameState.bombs[i].position.z;

            XZ = diffX + diffZ;

            if (XZ < minXZ)
            {
                minDiffX = diffX;
                minDiffZ = diffZ;
                minIndex = i;
                minXZ = XZ;
            }

            if (!((nextGameState.bombs[i].position.x > nextGameState.iaPosition.x + 1)      // trop à droite
            || (nextGameState.bombs[i].position.x + 1 <= nextGameState.iaPosition.x) // trop à gauche
            || (nextGameState.bombs[i].position.z >= nextGameState.iaPosition.z + 1) // trop en bas
            || (nextGameState.bombs[i].position.z + 1 <= nextGameState.iaPosition.z)))
            {
                nextGameState.minDistToIA = 0;
                return;
            }

            BombCollideWithWall(i, nextGameState);
            
        }

        nextGameState.minDistToIA = minXZ;//Mathf.Sqrt(Mathf.Pow((nextGameState.bombs[minIndex].position.x - nextGameState.iaPosition.x), 2)
                                          //              + Mathf.Pow((nextGameState.bombs[minIndex].position.z - nextGameState.iaPosition.z), 2));
    }

    public bool collision(Transform t1, Transform t2)
    {
        return !((t1.position.x > t2.position.x + t2.localScale.x)      // trop à droite
            || (t1.position.x + t1.localScale.x <= t2.position.x) // trop à gauche
            || (t1.position.z >= t2.position.z + t2.localScale.z) // trop en bas
            || (t1.position.z + t1.localScale.z <= t2.position.z));
    }

    public bool collisionIA(Vector3 p1, Transform t2)
    {
        return !((p1.x > t2.position.x + t2.localScale.x)      // trop à droite
            || (p1.x + 1 <= t2.position.x) // trop à gauche
            || (p1.z >= t2.position.z + t2.localScale.z) // trop en bas
            || (p1.z + 1 <= t2.position.z));
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

    /*private void BombCollideWithBomb(int i, int j, BombInfo[] bombs)
    {
        var tmpDir = bombs[i].direction;
        bombs[i].direction = bombs[j].direction;
        bombs[j].direction = tmpDir;

        bombManagers[i].dx = bombs[i].direction.x;
        bombManagers[i].dz = bombs[i].direction.z;
        bombManagers[j].dx = bombs[j].direction.x;
        bombManagers[j].dz = bombs[j].direction.z;
    }*/

    /*private void BombCollideWithWall(int i, GameState gs)
    {
        if (gs.bombs[i].position.x < bottomWall + bombRadius)
        {
            gs.bombs[i].direction = IAScript.InvDirection[gs.bombs[i].direction * 4 + 3];
            gs.bombs[i].position.x = bottomWall + bombRadius;
        }
        else if (gs.bombs[i].position.x > topWall - bombRadius)
        {
            gs.bombs[i].direction = IAScript.InvDirection[gs.bombs[i].direction * 4 + 1];
            gs.bombs[i].position.x = topWall - bombRadius;
        }
        else if (gs.bombs[i].position.z > rightWall - bombRadius)
        {
            gs.bombs[i].direction = IAScript.InvDirection[gs.bombs[i].direction * 4 ];
            gs.bombs[i].position.z = rightWall - bombRadius;
        }
        else if (gs.bombs[i].position.z < leftWall + bombRadius)
        {
            gs.bombs[i].direction = IAScript.InvDirection[gs.bombs[i].direction * 4 + 2];
            gs.bombs[i].position.z = leftWall + bombRadius;
        }
    }*/

    private void BombCollideWithWall(int i, GameState gs)
    {
        if (gs.bombs[i].position.x < bottomWall + bombRadius)
        {
            gs.bombs[i].direction.x = -gs.bombs[i].direction.x;
            gs.bombs[i].position.x = bottomWall + bombRadius;
        }
        else if (gs.bombs[i].position.x > topWall - bombRadius)
        {
            gs.bombs[i].direction.x = - gs.bombs[i].direction.x ;
            gs.bombs[i].position.x = topWall - bombRadius;
        }
        else if (gs.bombs[i].position.z > rightWall - bombRadius)
        {
            gs.bombs[i].direction.z = -gs.bombs[i].direction.z;
            gs.bombs[i].position.z = rightWall - bombRadius;
        }
        else if (gs.bombs[i].position.z < leftWall + bombRadius)
        {
            gs.bombs[i].direction.z = -gs.bombs[i].direction.z;
            gs.bombs[i].position.z = leftWall + bombRadius;
        }
    }
}
