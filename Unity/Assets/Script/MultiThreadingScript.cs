using UnityEngine;
using System.Collections;

public class MultiThreadingScript : MonoBehaviour {

    IACallBack callback;

    [SerializeField]
    LongTermScript longTermScript;

    public MultiThreadingScript(IACallBack callback)
    {
        this.callback = callback;
    }

    public void GetCheckPoints()
    {
        var checkPoints = longTermScript.FindAICheckPoints();

        callback(checkPoints);
    }
}

public delegate void IACallBack(Vector3[] checkPoints);


