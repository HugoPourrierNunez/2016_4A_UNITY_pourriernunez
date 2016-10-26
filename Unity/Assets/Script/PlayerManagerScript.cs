using UnityEngine;
using System.Collections;

public class PlayerManagerScript : MonoBehaviour {

    GameManagerScript gameManagerScript;

    public void setGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }



    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        SelectObject();
	}

    void SelectObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int j=0;//TODO : takeoff
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                for(int i = 0; i < gameManagerScript.CollisionManagerScript.bombManagers.Length; i++)
                {
                    if (gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.Equals(hit.transform.gameObject))
                    {
                        gameManagerScript.ActualGameState.bombs[i].state = BombState.Explosion;
                        j = i;
                    }
                }
                Debug.Log("You selected the " + hit.transform.name + " correspondant à " + gameManagerScript.CollisionManagerScript.bombManagers[j]); // ensure you picked right object
                Debug.Log(gameManagerScript.ActualGameState.bombs[j].state);
            }
        }
    }


}
