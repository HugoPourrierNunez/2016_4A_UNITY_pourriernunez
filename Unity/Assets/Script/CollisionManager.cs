using UnityEngine;
using System.Collections;

public class CollisionManager : MonoBehaviour
{

    private struct Bomb
    {
        public float x0;
        public float z0;
        public float dx;
        public float dz;
        public float t;
    }

    private enum Walls
    {
        bottom,
        top,
        right,
        left
    }


    [SerializeField]
    public BombManager[] bombManagers;

    private int nbBombs;
    private float bombRadius = 0.5f;
    private float bottomWall = -20.0f;
    private float topWall = 20.0f;
    private float rightWall = 20.0f;
    private float leftWall = -20.0f;
    private Bomb[] bombs;
    private float[,] bombsDistance;

	void OnEnable ()
    {
        nbBombs = bombManagers.Length;

        bombs = new Bomb[nbBombs];
        bombsDistance = new float[nbBombs , nbBombs];

        for (var i = 0; i < nbBombs; i++)
        {
            bombs[i].x0 = bombManagers[i].x;
            bombs[i].z0 = bombManagers[i].z;
            bombs[i].dx = bombManagers[i].dx;
            bombs[i].dz = bombManagers[i].dz;
        }

        for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance[i,j] = Mathf.Sqrt(Mathf.Pow((bombs[i].x0 - bombs[j].x0), 2) 
                                        + Mathf.Pow((bombs[i].z0 - bombs[j].z0), 2));
            }
        }
    }

    void Update()
    {
        for (var i = 0; i < nbBombs; i++)
        {
            bombs[i].t += 4 * Time.deltaTime;
        }

        for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance[i,j] = Mathf.Sqrt(Mathf.Pow((bombs[i].x0 + bombs[i].t * bombs[i].dx - bombs[j].x0 - bombs[j].t * bombs[j].dx), 2)
                                        + Mathf.Pow((bombs[i].z0 + bombs[i].t * bombs[i].dz - bombs[j].z0 - bombs[j].t * bombs[j].dz), 2));
                
                if (bombsDistance[i,j] < 1)
                {
                    BombCollideWithBomb(i, j);
                }
            }

            if (bombs[i].x0 + bombs[i].t * bombs[i].dx < bottomWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.bottom);
            }

            else if (bombs[i].x0 + bombs[i].t * bombs[i].dx > topWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.top);
            }

            else if (bombs[i].z0 + bombs[i].t * bombs[i].dz > rightWall - bombRadius)
            {
                BombCollideWithWall(i, Walls.right);
            }

            else if (bombs[i].z0 + bombs[i].t * bombs[i].dz < leftWall + bombRadius)
            {
                BombCollideWithWall(i, Walls.left);
            }
        }
    }

    void BombCollideWithBomb(int i, int j)
    {
        bombs[i].x0 += bombs[i].t * bombs[i].dx;
        bombs[i].z0 += bombs[i].t * bombs[i].dz;
        bombs[j].x0 += bombs[j].t * bombs[j].dx;
        bombs[j].z0 += bombs[j].t * bombs[j].dz;

        var tmpDx = bombs[i].dx;
        var tmpDy = bombs[i].dz;
        bombs[i].dx = bombs[j].dx;
        bombs[i].dz = bombs[j].dz;
        bombs[j].dx = tmpDx;
        bombs[j].dz = tmpDy;

        bombs[i].t = 0;
        bombs[j].t = 0;

        bombManagers[i].dx = bombs[i].dx;
        bombManagers[i].dz = bombs[i].dz;
        bombManagers[j].dx = bombs[j].dx;
        bombManagers[j].dz = bombs[j].dz;
    }

    void BombCollideWithWall(int i, Walls wall)
    {
        bombs[i].x0 += bombs[i].t * bombs[i].dx;
        bombs[i].z0 += bombs[i].t * bombs[i].dz;

        switch (wall)
        {
            case Walls.bottom:
                bombs[i].dx = Mathf.Abs(bombs[i].dx);
                break;
            case Walls.top:
                bombs[i].dx = -Mathf.Abs(bombs[i].dx);
                break;
            case Walls.right:
                bombs[i].dz = -Mathf.Abs(bombs[i].dz);
                break;
            case Walls.left:
                bombs[i].dz = Mathf.Abs(bombs[i].dz);
                break;
        }

        bombs[i].t = 0;

        bombManagers[i].dx = bombs[i].dx;
        bombManagers[i].dz = bombs[i].dz;
    }
}
