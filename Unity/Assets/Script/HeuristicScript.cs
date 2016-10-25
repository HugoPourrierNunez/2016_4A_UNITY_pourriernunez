using UnityEngine;
using System.Collections;

public class HeuristicScript  {

    public float GetShortHeuristic(GameState state)
    {
        float minDistance = 1000000;
        var length = state.bombes.Length;
        for (var i=0;i<length;i++)
        {
            float dist = Mathf.Pow(state.bombes[i].position.x - state.iaPosition.x, 2) + Mathf.Pow(state.bombes[i].position.z - state.iaPosition.z, 2);
            if(dist < minDistance)
            {
                minDistance = dist;
            }
        }
        return 1/minDistance;
    }

}
