using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour {

    GameState actualGameState;

    [SerializeField]
    CollisionManagerScript collisionManagerScript;

    [SerializeField]
    MapManagerScript mapManagerScript;

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

    void Start()
    {
        mapManagerScript.setGameManagerScript(this);
        collisionManagerScript.setGameManagerScript(this);
        initializeGameState(CollisionManagerScript.getBombManagers());
    } 

    void initializeGameState(BombManagerScript[] bombManagers)
    {
        actualGameState = new GameState();
        var length = bombManagers.Length;
        actualGameState.bombs = new BombInfo[length];
        for(var i=0;i<length; i++)
        {
            bombManagers[i].initialize();
            actualGameState.bombs[i].position = bombManagers[i].transform.position;
            actualGameState.bombs[i].state = BombState.Normal;
            actualGameState.bombs[i].delay = -1;
            actualGameState.bombs[i].direction = bombManagers[i].getDirection(); 
        }
        
    }
}
