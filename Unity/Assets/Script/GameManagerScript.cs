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
    IAManagerScript iaManagerScript;

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

    public IAManagerScript IaManagerScript
    {
        get
        {
            return iaManagerScript;
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

    void Start()
    {
        mapManagerScript.setGameManagerScript(this);
        collisionManagerScript.setGameManagerScript(this);
        stateManagerScript.setGameManagerScript(this);
        iaManagerScript.setGameManagerScript(this);

        initializeGameState();
    }

    public void initializeGameState()
    {
        //Debug.Log("init gamestate");
        actualGameState = new GameState();

        iaManagerScript.getIaTransform().position = new Vector3(-19, .5f, -19);
        actualGameState.bombs = collisionManagerScript.InitializeBombInfo();
    }
}
