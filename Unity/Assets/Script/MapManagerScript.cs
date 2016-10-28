using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManagerScript : MonoBehaviour
{
    GameManagerScript gameManagerScript;

    [SerializeField]
    Transform plane;

    [SerializeField]
    Transform goal;

    [SerializeField]
    ObstacleScript[] obstacles;

    [SerializeField]
    Transform start;

    public void SetGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    public Transform GetGoalTransform()
    {
        return goal;
    }

    public Transform GetStartTransform()
    {
        return start;
    }


    public Transform GetPlaneTransform()
    {
        return plane;
    }
}
