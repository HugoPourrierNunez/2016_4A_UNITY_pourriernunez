using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManagerScript : MonoBehaviour{

    [SerializeField]
    Plane plane;

    [SerializeField]
    Transform entry;

    [SerializeField]
    Transform goal;

    [SerializeField]
    List<ObstacleScript> obstacles; 
}
