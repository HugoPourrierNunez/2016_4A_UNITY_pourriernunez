using UnityEngine;
using System.Collections;

public class ScriptManager : MonoBehaviour {

    [SerializeField]
    public GameObject collision;

    [SerializeField]
    public GameObject[] bombs;

	// Use this for initialization
	void Start ()
    {
        foreach (GameObject bomb in bombs)
        {
            bomb.SetActive(true);
        }
        collision.SetActive(true);
    }
}
