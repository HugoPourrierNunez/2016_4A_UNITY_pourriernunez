using UnityEngine;
using System.Collections;

public class IAManagerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void MoveController()
    {
        if (Input.GetKey("Up") && !Input.GetKey("Right") && !Input.GetKey("Left"))
        {
            // HAUT
        }

        if (Input.GetKey("Down") && !Input.GetKey("Right") && !Input.GetKey("Left"))
        {
            // BAS
        }

        if (Input.GetKey("Right") && !Input.GetKey("Down") && !Input.GetKey("Down"))
        {
            // DROITE
        }

        if (Input.GetKey("Left") && !Input.GetKey("Down") && !Input.GetKey("Down"))
        {
            // GAUCHE
        }

        if (Input.GetKey("Up") && Input.GetKey("Right") && !Input.GetKey("Left"))
        {
            // HAUT DROITE
        }

        if (Input.GetKey("Up") && !Input.GetKey("Right") && Input.GetKey("Left"))
        {
            // HAUT GAUCHE
        }

        if (Input.GetKey("Down") && Input.GetKey("Right") && !Input.GetKey("Left"))
        {
            // BAS DROITE
        }

        if (Input.GetKey("Down") && !Input.GetKey("Right") && Input.GetKey("Left"))
        {
            // BAS GAUCHE
        }
        
    }
}
