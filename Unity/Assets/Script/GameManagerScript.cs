using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour {

    GameState actualGameState;

    [SerializeField]
    CollisionManagerScript collisionManager;

    [SerializeField]
    MapManagerScript mapManager;
    
    void Start()
    {
        initializeGameState(collisionManager.getBombManagers());
    } 

    void initializeGameState(BombManagerScript[] bombManagers)
    {
        actualGameState = new GameState();
        var length = bombManagers.Length;
        actualGameState.bombes = new BombInfo[length];
        for(var i=0;i<length; i++)
        {
            bombManagers[i].initialize();
            actualGameState.bombes[i].position = bombManagers[i].transform.position;
            actualGameState.bombes[i].state = BombState.Normal;
            actualGameState.bombes[i].delay = -1;
            actualGameState.bombes[i].direction = bombManagers[i].getDirection();

        }
        
    }
}
