using UnityEngine;
using System.Collections;

public class PlayerManagerScript : MonoBehaviour
{

    GameManagerScript gameManagerScript;
    private BombManagerScript bombSelected;
    private int indiceBombSelected;

    public void SetGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    void Start()
    {
        indiceBombSelected = 0;
        bombSelected = gameManagerScript.CollisionManagerScript.bombManagers[indiceBombSelected];
        bombSelected.ApplyColor(Color.yellow);
    }



	void Update ()
    {
        TriggerChangeSelectedBomb();
        TriggerExplosion();
		TriggerLaser();
    }

    void TriggerExplosion()
    {
        if (Input.GetKeyDown(KeyCode.Space) && indiceBombSelected!=-1)
        {
            if (gameManagerScript.ActualGameState.bombs[indiceBombSelected].state != BombState.Explosion)
            {
                gameManagerScript.ActualGameState.bombs[indiceBombSelected].state = BombState.Explosion;
                gameManagerScript.ActualGameState.bombs[indiceBombSelected].delay = 2000;

                var i = 1;
                var length = gameManagerScript.CollisionManagerScript.bombManagers.Length;
                indiceBombSelected++;
                if (indiceBombSelected==length)
                    indiceBombSelected=0;
                while(i<length && gameManagerScript.ActualGameState.bombs[indiceBombSelected].state != BombState.Normal)
                {
                    indiceBombSelected++;
                    i++;
                    if (indiceBombSelected == length)
                        indiceBombSelected=0;
                }
                if (i == length)
                    indiceBombSelected = -1;
                else
                {
                    bombSelected = gameManagerScript.CollisionManagerScript.bombManagers[indiceBombSelected];
                    bombSelected.ApplyColor(Color.yellow);
                }

            }
         }
    }

    void TriggerChangeSelectedBomb()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            var length = gameManagerScript.CollisionManagerScript.bombManagers.Length;
            if (indiceBombSelected == -1)
                indiceBombSelected = length-1;
            else bombSelected.ApplyColor(Color.black);

            indiceBombSelected++;
            if (indiceBombSelected == length)
                indiceBombSelected++;
            while (gameManagerScript.ActualGameState.bombs[indiceBombSelected].state != BombState.Normal)
            {
                indiceBombSelected++;
                if (indiceBombSelected == length)
                    indiceBombSelected++;
            }
            bombSelected = gameManagerScript.CollisionManagerScript.bombManagers[indiceBombSelected];
            bombSelected.ApplyColor(Color.yellow);
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var length = gameManagerScript.CollisionManagerScript.bombManagers.Length;
            if (indiceBombSelected == -1)
                indiceBombSelected = 0;
            else bombSelected.ApplyColor(Color.black);

            indiceBombSelected--;
            if (indiceBombSelected == -1)
                indiceBombSelected=length-1;
            while (gameManagerScript.ActualGameState.bombs[indiceBombSelected].state != BombState.Normal)
            {
                indiceBombSelected--;
                if (indiceBombSelected == -1)
                    indiceBombSelected = length - 1;
            }
            bombSelected = gameManagerScript.CollisionManagerScript.bombManagers[indiceBombSelected];
            bombSelected.ApplyColor(Color.yellow);
        }

        
    }

    void TriggerLaser()
    {
        if (Input.GetKeyDown(KeyCode.Return) && indiceBombSelected != -1)
        {
            if (gameManagerScript.ActualGameState.bombs[indiceBombSelected].state == BombState.Normal)
            {
                gameManagerScript.ActualGameState.bombs[indiceBombSelected].state = BombState.Laser;
                gameManagerScript.ActualGameState.bombs[indiceBombSelected].delay = 2000;

                var i = 1;
                var length = gameManagerScript.CollisionManagerScript.bombManagers.Length;
                indiceBombSelected++;
                if (indiceBombSelected == length)
                    indiceBombSelected = 0;
                while (i < length && gameManagerScript.ActualGameState.bombs[indiceBombSelected].state != BombState.Normal)
                {
                    indiceBombSelected++;
                    i++;
                    if (indiceBombSelected == length)
                        indiceBombSelected = 0;
                }
                if (i == length)
                    indiceBombSelected = -1;
                else
                {
                    bombSelected = gameManagerScript.CollisionManagerScript.bombManagers[indiceBombSelected];
                    bombSelected.ApplyColor(Color.yellow);
                }

            }
        }
    }

    /*void TriggerLaser()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                for (var i = 0; i < gameManagerScript.CollisionManagerScript.bombManagers.Length; i++)
                {
                    if (gameManagerScript.CollisionManagerScript.bombManagers[i].gameObject.Equals(hit.transform.gameObject))
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
    }*/
}
