using UnityEngine;
using System.Collections;

public class IAManagerScript : MonoBehaviour {

    [SerializeField]
    private Transform transformPlayer;

    [SerializeField]
    private Renderer rendererPlayer;

    [SerializeField]
    private float speedIA = 1f;

    [SerializeField]
    private Renderer planeRenderer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        MoveController();
        CollisionIA();
	}

    void MoveController()
    {
        if (Input.GetKey("up") && !Input.GetKey("right") && !Input.GetKey("left"))
        {
            // HAUT
            transformPlayer.position = new Vector3(transformPlayer.position.x, transformPlayer.position.y, transformPlayer.position.z + speedIA);
        }
        else if (Input.GetKey("down") && !Input.GetKey("right") && !Input.GetKey("left"))
        {
            // BAS
            transformPlayer.position = new Vector3(transformPlayer.position.x, transformPlayer.position.y, transformPlayer.position.z - speedIA);
        }
        else if (Input.GetKey("right") && !Input.GetKey("down") && !Input.GetKey("up"))
        {
            // DROITE
            transformPlayer.position = new Vector3(transformPlayer.position.x + speedIA, transformPlayer.position.y, transformPlayer.position.z);
        }
        else if (Input.GetKey("left") && !Input.GetKey("down") && !Input.GetKey("up"))
        {
            // GAUCHE
            transformPlayer.position = new Vector3(transformPlayer.position.x - speedIA, transformPlayer.position.y, transformPlayer.position.z);
        }
        else if (Input.GetKey("up") && Input.GetKey("right") && !Input.GetKey("left"))
        {
            // HAUT DROITE
            transformPlayer.position = new Vector3(transformPlayer.position.x + speedIA, transformPlayer.position.y, transformPlayer.position.z + speedIA);
        }
        else if (Input.GetKey("up") && !Input.GetKey("right") && Input.GetKey("left"))
        {
            // HAUT GAUCHE
            transformPlayer.position = new Vector3(transformPlayer.position.x - speedIA, transformPlayer.position.y, transformPlayer.position.z - speedIA);
        }
        else if (Input.GetKey("down") && Input.GetKey("right") && !Input.GetKey("left"))
        {
            // BAS DROITE
            transformPlayer.position = new Vector3(transformPlayer.position.x + speedIA, transformPlayer.position.y, transformPlayer.position.z - speedIA);
        }
        else if (Input.GetKey("down") && !Input.GetKey("right") && Input.GetKey("left"))
        {
            // BAS GAUCHE
            transformPlayer.position = new Vector3(transformPlayer.position.x - speedIA, transformPlayer.position.y, transformPlayer.position.z - speedIA);
        }
    }

    bool CollisionIA()
    {
        float maxPlaneX = planeRenderer.bounds.size.x / 2;
        float minPlaneX = -maxPlaneX;

        float maxPlaneY = planeRenderer.bounds.size.z / 2;
        float minPlaneY = -maxPlaneY;

        float sizeDivise = rendererPlayer.bounds.size.x / 2;

        // TEST MUR
        if (transformPlayer.position.x - sizeDivise >= maxPlaneX || transformPlayer.position.x - sizeDivise <= minPlaneX
            || transformPlayer.position.z - sizeDivise > maxPlaneY || transformPlayer.position.z - sizeDivise < minPlaneY)
        {
            Debug.Log("Collision");
            return true;
        }

        // TEST BOMBE

        return false;
    }
}
