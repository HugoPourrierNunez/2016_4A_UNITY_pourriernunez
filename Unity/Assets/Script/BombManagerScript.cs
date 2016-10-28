using UnityEngine;
using System.Collections;

public class BombManagerScript : MonoBehaviour
{
    [SerializeField]
    GameObject bombGO;

    [SerializeField]
    Renderer rendererBomb;

    [SerializeField]
    GameObject explosion;

    [SerializeField]
    GameObject laser1;

    [SerializeField]
    GameObject laser2;

    public void SetScaleLaser(Transform plane)
    {
        laser1.transform.localScale = new Vector3(plane.localScale.x * 10, 1 ,1);
        laser2.transform.localScale = new Vector3(1, 1, plane.localScale.z * 10);
    }

    public void ApplyBombInfo(BombInfo bombInfo)
    {
        bombGO.transform.position = bombInfo.position;

        laser1.transform.localPosition = new Vector3(-bombGO.transform.position.x, 0, 0);
        laser2.transform.localPosition = new Vector3(0, 0, -bombGO.transform.position.z);

        if (bombInfo.state == BombState.Explosion)
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

            if(bombInfo.delay<=0)
            {
                bombInfo.state = BombState.BOOM;
                bombInfo.delay = 300;
                explosion.SetActive(true);
            }
        }
        if (bombInfo.state == BombState.Laser)
        {
            var time = bombInfo.delay % 50f;
            if (time > 25)
            {
                rendererBomb.material.color = Color.cyan;
            }
            else
            {
                rendererBomb.material.color = Color.black;
            }

            if (bombInfo.delay <= 0)
            {
                bombInfo.state = BombState.BZZZ;
                bombInfo.delay = 700;
                ActiveLaser();
            }
        }
        else if(bombInfo.state == BombState.BOOM && bombInfo.delay<=0)
        {
            bombInfo.state = BombState.Normal;
            explosion.SetActive(false);
        }
        else if(bombInfo.state == BombState.BZZZ && bombInfo.delay <= 0)
        {
            bombInfo.state = BombState.Normal;
            laser1.SetActive(false);
            laser2.SetActive(false);
        }


    }

    public void ResetState()
    {
        laser1.SetActive(false);
        laser2.SetActive(false);
        explosion.SetActive(false);
        ApplyColor(Color.black);
    }

    public void ApplyColor(Color c)
    {
        rendererBomb.material.color = c;
    }

    public void ActiveLaser()
    { 
        laser1.SetActive(true);
        laser2.SetActive(true);
    }
}
