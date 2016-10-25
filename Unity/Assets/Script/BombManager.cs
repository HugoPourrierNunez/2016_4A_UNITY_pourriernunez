using UnityEngine;
using System.Collections;

public class BombManager : MonoBehaviour
{

    [SerializeField]
    public Transform transform;

    public float x { get; set; }
    public float z { get; set; }
    public float dx { get; set; }
    public float dz { get; set; }

    void OnEnable()
    {
        this.x = transform.position.x;
        this.z = transform.position.z;

        int random = Random.Range(1, 9);

        switch (random)
        {
            case 1:
                this.dx = 0.0f;
                this.dz = 1.0f;
                break;
            case 2:
                this.dx = Mathf.Sqrt(2.0f);
                this.dz = Mathf.Sqrt(2.0f);
                break;
            case 3:
                this.dx = 1.0f;
                this.dz = 0.0f;
                break;
            case 4:
                this.dx = Mathf.Sqrt(2.0f);
                this.dz = -Mathf.Sqrt(2.0f);
                break;
            case 5:
                this.dx = 0.0f;
                this.dz = -1.0f;
                break;
            case 6:
                this.dx = -Mathf.Sqrt(2.0f);
                this.dz = -Mathf.Sqrt(2.0f);
                break;
            case 7:
                this.dx = -1.0f;
                this.dz = 0.0f;
                break;
            case 8:
                this.dx = Mathf.Sqrt(2.0f);
                this.dz = -Mathf.Sqrt(2.0f);
                break;
        }
    }

    void Update()
    {
        this.x += 4 * Time.deltaTime * dx;
        this.z += 4 * Time.deltaTime * dz;

        Vector3 vect = new Vector3(4 * Time.deltaTime * dx, 0.0f, 4 * Time.deltaTime * dz);
        transform.position += vect;
    }
}
