using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    // Update is called once per frame
    void Update ()
    {
        // Attache le rayon d'explosion de la bombe à la bombe
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, transform.parent.position.z);
    }
}
