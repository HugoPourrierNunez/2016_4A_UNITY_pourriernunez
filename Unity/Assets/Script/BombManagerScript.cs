using UnityEngine;
using System.Collections;

public class BombManagerScript : MonoBehaviour
{
    [SerializeField]
    GameObject bombGO;

    [SerializeField]
    Renderer rendererBomb;
    
    public void ApplyBombInfo(BombInfo bombInfo)
    {
        bombGO.transform.position = bombInfo.position;

        if(bombInfo.state == BombState.Explosion)
        {
            var time = bombInfo.delay % 50f; 
            if(time > 25)
            {
                rendererBomb.material.color = Color.red;
            }
            else
            {
                rendererBomb.material.color = Color.black;
            }
        }
    }
}
