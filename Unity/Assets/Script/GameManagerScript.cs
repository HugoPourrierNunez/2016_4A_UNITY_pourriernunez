using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour {

    GameState actualGameState;

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

    void Start()
    {
        mapManagerScript.setGameManagerScript(this);
        collisionManagerScript.setGameManagerScript(this);
        stateManagerScript.setGameManagerScript(this);
        playerManagerScript.setGameManagerScript(this);

        if(iAScript!=null)
            iAScript.setGameManagerScript(this);

        initializeGameState();
    }

    public void initializeGameState()
    {
        //Debug.Log("init gamestate");
        actualGameState = new GameState(CollisionManagerScript.getBombManagers().Length);

        actualGameState.iaPosition = new Vector3(-19, .5f, -19);
        collisionManagerScript.InitializeBombInfo(actualGameState);
        actualGameState.timeSinceStart = Time.time * 1000;
    }
}
