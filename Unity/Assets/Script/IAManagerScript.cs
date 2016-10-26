using UnityEngine;
using System.Collections;

public class IAManagerScript : MonoBehaviour
{

    [SerializeField]
    private Transform transformPlayer;

    [SerializeField]
    private Renderer rendererPlayer;

    [SerializeField]
    private float speedIA = 1f;

    [SerializeField]
    private Renderer planeRenderer;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveController();
    }

    void MoveController()
    {
        var iaMoveDirection = new Vector3(0.0f, 0.0f, 0.0f);

        var maxPlaneX = planeRenderer.bounds.size.x / 2;
        var minPlaneX = -maxPlaneX;

        var maxPlaneY = planeRenderer.bounds.size.z / 2;
        var minPlaneY = -maxPlaneY;

        var sizeDivise = rendererPlayer.bounds.size.x / 2;

        if (Input.GetKey("up"))
        {
            iaMoveDirection.z = speedIA;
        }
        if (Input.GetKey("down"))
        {
            iaMoveDirection.z = -speedIA;
        }
        if (Input.GetKey("right"))
        {
            iaMoveDirection.x = speedIA;
        }
        if (Input.GetKey("left"))
        {
            iaMoveDirection.x = -speedIA;
        }


        if (transformPlayer.position.x + iaMoveDirection.x + sizeDivise >= maxPlaneX || transformPlayer.position.x + iaMoveDirection.x - sizeDivise <= minPlaneX)
        {
            iaMoveDirection.x = 0.0f;
        }

        if (transformPlayer.position.z + iaMoveDirection.z + sizeDivise >= maxPlaneY || transformPlayer.position.z + iaMoveDirection.z - sizeDivise <= minPlaneY)
        {
            iaMoveDirection.z = 0.0f;
        }

        transformPlayer.position += iaMoveDirection;
    }
}
