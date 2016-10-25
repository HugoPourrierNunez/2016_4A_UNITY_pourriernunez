using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {

    public Transform transformIA;
    public float speedIA = 0.1f;


    void moveRight()
    {
        transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z);
    }
    void moveUp()
    {
        transformIA.position = new Vector3(transformIA.position.x, transformIA.position.y + speedIA, transformIA.position.z);
    }
    void moveLeft()
    {
        transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y, transformIA.position.z);
    }
    void moveDown()
    {
        transformIA.position = new Vector3(transformIA.position.x, transformIA.position.y - speedIA, transformIA.position.z);
    }
    void moveUpRight()
    {
        transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y + speedIA, transformIA.position.z);
    }
    void moveUpLeft()
    {
        transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y + speedIA, transformIA.position.z);
    }
    void moveDownRight()
    {
        transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y - speedIA, transformIA.position.z);
    }
    void moveDownLeft()
    {
        transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y - speedIA, transformIA.position.z);
    }
}
