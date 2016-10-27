﻿using UnityEngine;
using System.Collections;

public class PlayerManagerScript : MonoBehaviour
{

    GameManagerScript gameManagerScript;
    private GameObject firstBombChoosen;
    private LineRenderer lineX;
    private LineRenderer lineY;

    public void SetGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    void Start()
    {
        this.firstBombChoosen = null;
        lineX = GetComponent<LineRenderer>();
        lineY = GetComponent<LineRenderer>();
    }

	void Update ()
    {
        TriggerExplosion();
		TriggerLaser();
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

    void TriggerLaser()
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
    }
}
