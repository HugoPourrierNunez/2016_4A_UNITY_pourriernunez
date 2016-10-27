using UnityEngine;
using System.Collections;

public class GameState  {

    public Vector3 iaPosition;
    public Vector3 iaDirection;
    public BombInfo[] bombs;
    public float timeSinceStart;
    public float minDistToIA;
    public float score;
    public Vector3 originalDirection;

    public GameState(int nbBombs)
    {
        iaPosition = Vector3.zero;
        iaDirection = Vector3.zero;
        timeSinceStart = 0;
        minDistToIA = 0;
        score = -1;
        bombs = new BombInfo[nbBombs];
        originalDirection = Vector3.zero;

        for(var i=0;i< nbBombs; i++)
        {
            bombs[i] = new BombInfo();
        }
    }

    public void Copy(GameState gs)
    {
        gs.iaDirection = iaDirection;
        gs.iaPosition = iaPosition;
        gs.minDistToIA = minDistToIA;
        gs.timeSinceStart = timeSinceStart;
        gs.score = score;
        var length = bombs.Length;
        for (var i = 0; i < length; i++)
        {
            bombs[i].Copy(gs.bombs[i]);
        }
    }
    
}

public class BombInfo
{
    public Vector3 position;
    public float delay;
    public Vector3 direction;
    public BombState state;

    public BombInfo()
    {
        position = Vector3.zero;
        delay = 0;
        direction = Vector3.zero;
        state = BombState.Normal;
    }

    public void Copy(BombInfo bi)
    {
        bi.position = position;
        bi.delay = delay;
        bi.direction = direction;
        bi.state = state;
    }

}

public enum BombState { Normal, Explosion, Laser };

