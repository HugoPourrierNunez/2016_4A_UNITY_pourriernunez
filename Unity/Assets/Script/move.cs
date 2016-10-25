using UnityEngine;
using System.Collections;

public class move : MonoBehaviour
{
    public Transform transformIA;
    public float speedIA = 0.1f;

    void moveIA(int i)
    {
        switch (i)
        {
            case 1://up
                transformIA.position = new Vector3(transformIA.position.x, transformIA.position.y, transformIA.position.z + speedIA);
                break;
            case 2://up right
                transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z + speedIA);
                break;
            case 3://right
                transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z);
                break;
            case 4://down right
                transformIA.position = new Vector3(transformIA.position.x + speedIA, transformIA.position.y, transformIA.position.z - speedIA);
                break;
            case 5://down
                transformIA.position = new Vector3(transformIA.position.x, transformIA.position.y, transformIA.position.z - speedIA);
                break;
            case 6://down left
                transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y, transformIA.position.z - speedIA);
                break;
            case 7://left
                transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y, transformIA.position.z);
                break;
            case 8://up left 
                transformIA.position = new Vector3(transformIA.position.x - speedIA, transformIA.position.y, transformIA.position.z + speedIA);
                break;
        }
    }
    public int GetLongHeuristic(GameState state)
    {

        return 0;
    }
}
