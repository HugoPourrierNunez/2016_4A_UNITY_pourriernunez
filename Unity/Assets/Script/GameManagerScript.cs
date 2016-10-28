using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BombState
{
    Normal,
    Explosion,
    BOOM
}

public class GameManagerScript : MonoBehaviour {

    public GameState actualGameState;

    [SerializeField]
    CollisionManagerScript collisionManagerScript;

    [SerializeField]
    MapManagerScript mapManagerScript;

    [SerializeField]
    StateManagerScript stateManagerScript;

    [SerializeField]
    PlayerManagerScript playerManagerScript;

    [SerializeField]
    IAScript iAScript;

    [SerializeField]
    LongTermScript longTermScript;

    [SerializeField]
    AStarGridScript aStarGridScript;

    void Start()
    {
        mapManagerScript.SetGameManagerScript(this);
        collisionManagerScript.SetGameManagerScript(this);
        stateManagerScript.SetGameManagerScript(this);
        playerManagerScript.SetGameManagerScript(this);
        //longTermScript.SetGameManagerScript(this);
        aStarGridScript.SetGameManagerScript(this);

        if (iAScript != null)
        {
            iAScript.SetGameManagerScript(this);
        }

        InitializeGameState();

        //////////////////////////////////////////////////
        //              Test multi-threading            //
        //////////////////////////////////////////////////



    }

    public void InitializeGameState()
    {
        actualGameState = new GameState(CollisionManagerScript.getBombManagers().Length);

        actualGameState.iaPosition = new Vector3(-19, .5f, -19);
        collisionManagerScript.InitializeBombInfo(actualGameState);
        actualGameState.timeSinceStart = Time.time * 1000;
    }

    public CollisionManagerScript CollisionManagerScript
    {
        get
        {
            return collisionManagerScript;
        }
    }

    public MapManagerScript MapManagerScript
    {
        get
        {
            return mapManagerScript;
        }
    }

    public StateManagerScript StateManagerScript
    {
        get
        {
            return stateManagerScript;
        }
    }

    public GameState ActualGameState
    {
        get
        {
            return actualGameState;
        }

        set
        {
            actualGameState = value;
        }
    }

    public IAScript IAScript
    {
        get
        {
            return iAScript;
        }
        
    }
}
