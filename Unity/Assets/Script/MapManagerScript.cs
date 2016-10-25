using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManagerScript : MonoBehaviour
{

    [SerializeField]
    Plane plane;

    [SerializeField]
    Transform entry;

    [SerializeField]
    Transform goal;

    [SerializeField]
    ObstacleScript[] obstacles;

    GameManagerScript gameManagerScript;

    public void setGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }
}
