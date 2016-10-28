using UnityEngine;
using System.Collections;

public class MultiThreadingScript : MonoBehaviour {

    IACallBack callback;
    LongTermScript longTermScript;
    Vector3 startPosition;
    Vector3 targetPosition;

    public MultiThreadingScript(IACallBack callback, LongTermScript longTermScript, Vector3 startPosition, Vector3 targetPosition)
    {
        this.callback = callback;
        this.longTermScript = longTermScript;
        this.startPosition = startPosition;
        this.targetPosition = targetPosition;
    }

    public void GetCheckPoints()
    {
        var checkPoints = longTermScript.FindAICheckPoints(startPosition, targetPosition);
        callback(checkPoints);
    }
}

public delegate void IACallBack(Vector3[] checkPoints);


