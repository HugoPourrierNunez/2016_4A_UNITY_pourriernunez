using UnityEngine;
using System.Collections;

public class BombManagerScript : MonoBehaviour
{

    [SerializeField]
    GameObject bombGO;


    public float x0 { get; set; }
    public float z0 { get; set; }
    public float dx { get; set; }
    public float dz { get; set; }

    public void initializeBomb(Transform plane)
    {
        /*this.x0 = transform.position.x;
        this.z0 = transform.position.z;*/

        this.x0 = Random.Range(plane.position.x - plane.localScale.x*5 +bombGO.transform.localScale.x/2, plane.position.x + plane.localScale.x *5 - bombGO.transform.localScale.x / 2);
        this.z0 = Random.Range(plane.position.z - plane.localScale.z *5 + bombGO.transform.localScale.z / 2, plane.position.z + plane.localScale.z * 5 - bombGO.transform.localScale.z / 2);

        int random = Random.Range(1, 9);

        switch (random)
        {
            case 1:
                this.dx = 0.0f;
                this.dz = 1.0f;
                break;
            case 2:
                this.dx = Mathf.Sqrt(0.5f);
                this.dz = Mathf.Sqrt(0.5f);
                break;
            case 3:
                this.dx = 1.0f;
                this.dz = 0.0f;
                break;
            case 4:
                this.dx = Mathf.Sqrt(0.5f);
                this.dz = -Mathf.Sqrt(0.5f);
                break;
            case 5:
                this.dx = 0.0f;
                this.dz = -1.0f;
                break;
            case 6:
                this.dx = -Mathf.Sqrt(0.5f);
                this.dz = -Mathf.Sqrt(0.5f);
                break;
            case 7:
                this.dx = -1.0f;
                this.dz = 0.0f;
                break;
            case 8:
                this.dx = Mathf.Sqrt(0.5f);
                this.dz = -Mathf.Sqrt(0.5f);
                break;
        }
    }

    /*void Update()
    {
        //Debug.Log("Update");
        Vector3 vect = new Vector3(4 * Time.deltaTime * dx, 0.0f, 4 * Time.deltaTime * dz);
        transform.position += vect;
    }*/

    public Vector2 getDirection()
    {
        return new Vector2(dx, dz);
    }

    public void ApplyBombInfo(BombInfo bombInfo)
    {
        bombGO.transform.position = bombInfo.position;
    }
}
