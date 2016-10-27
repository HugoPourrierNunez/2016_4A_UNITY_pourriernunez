using UnityEngine;
using System.Collections;

public class PlayerManagerScript : MonoBehaviour {

    GameManagerScript gameManagerScript;

    public void SetGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }
	
	void Update ()
    {
        TriggerExplosion();
	}

    void TriggerExplosion()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                for(var i = 0; i < gameManagerScript.CollisionManagerScript.bombManagers.Length; i++)
                {
                    if (gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.Equals(hit.transform.gameObject))
                    {
                        if(gameManagerScript.ActualGameState.bombs[i].state != BombState.Explosion)
                        {
                            gameManagerScript.ActualGameState.bombs[i].state = BombState.Explosion;
                            gameManagerScript.ActualGameState.bombs[i].delay = 2000;
                        }
                    }
                }
            }
        }
    }


}
