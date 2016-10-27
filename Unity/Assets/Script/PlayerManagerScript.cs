using UnityEngine;
using System.Collections;

public class PlayerManagerScript : MonoBehaviour
{

    GameManagerScript gameManagerScript;
    private GameObject firstBombChoosen;
    private LineRenderer lineX;
    private LineRenderer lineY;

    public void setGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    // Use this for initialization
    void Start()
    {
        this.firstBombChoosen = null;
        lineX = GetComponent<LineRenderer>();
        lineY = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        TriggerExplosion();
        TriggerLaser();
    }

    void TriggerExplosion()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))//récupère la cible de la souris
            {
                for (int i = 0; i < gameManagerScript.CollisionManagerScript.bombManagers.Length; i++)//parcours du tableau des bombes
                {
                    if (gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.Equals(hit.transform.gameObject))//identification de la cible dans le tableau
                    {
                        if (gameManagerScript.ActualGameState.bombs[i].state != BombState.Explosion)//vérifie que la bombe n'est pas déjà en train d'exploser
                        {
                            gameManagerScript.ActualGameState.bombs[i].state = BombState.Explosion;//mise à jour du bombstate
                            gameManagerScript.ActualGameState.bombs[i].delay = 2000;//initialisation du délai d'explosion
                        }
                    }
                }
            }
        }
    }

    void TriggerLaser()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))//récupère la cible de la souris
            {
                for (int i = 0; i < gameManagerScript.CollisionManagerScript.bombManagers.Length; i++)//parcours du tableau des bombes
                {
                    if (gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.Equals(hit.transform.gameObject))//identification de la cible dans le tableau
                    {
                        lineX.SetPosition(0, new Vector3(gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.transform.position.x, 0, 20));
                        lineX.SetPosition(1, new Vector3(gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.transform.position.x, 0, -20));
                        lineX.SetWidth(0.2f, 0.2f);
                        
                        lineY.SetPosition(0, new Vector3(20, 0, gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.transform.position.z));
                        lineY.SetPosition(1, new Vector3(-20, 0, gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.transform.position.z));
                        lineX.SetWidth(0.2f, 0.2f);
                    }
                }
            }
        }
    }
}
