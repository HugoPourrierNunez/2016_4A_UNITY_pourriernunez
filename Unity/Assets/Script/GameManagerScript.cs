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
            return StateManagerScript;
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

    void Start()
    {
        mapManagerScript.setGameManagerScript(this);
        collisionManagerScript.setGameManagerScript(this);
        stateManagerScript.setGameManagerScript(this);

        initializeGameState();
    }
    
    void OnEnabled()
    {
        initializeGameState();
    }

    void initializeGameState()
    {
        actualGameState = new GameState();
        actualGameState.bombs = collisionManagerScript.InitializeBombInfo();
    }
}
