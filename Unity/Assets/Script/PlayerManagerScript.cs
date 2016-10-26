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
        TriggerExplosion();
	}

    void TriggerExplosion()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))//récupère la cible de la souris
            {
                for(int i = 0; i < gameManagerScript.CollisionManagerScript.bombManagers.Length; i++)//parcours du tableau des bombes
                {
                    if (gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.Equals(hit.transform.gameObject))//identification de la cible dans le tableau
                    {
                        if(gameManagerScript.ActualGameState.bombs[i].state != BombState.Explosion)//vérifie que la bombe n'est pas déjà en train d'exploser
                        {
                            gameManagerScript.ActualGameState.bombs[i].state = BombState.Explosion;//mise à jour du bombstate
                            gameManagerScript.ActualGameState.bombs[i].delay = 2000;//initialisation du délai d'explosion
                        }
                    }
                }
            }
        }
    }


}
