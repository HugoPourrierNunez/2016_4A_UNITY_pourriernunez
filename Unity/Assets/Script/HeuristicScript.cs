using UnityEngine;
using System.Collections;

public class HeuristicScript
{
    public static float GetShortHeuristic(GameState state, Vector3 goal)
    {
        var minDistance = float.MaxValue;
        var length = state.bombs.Length;

        for (var i = 0; i < length; i++)
        {
            var dist = Mathf.Pow(state.bombs[i].position.x - state.iaPosition.x, 2) + Mathf.Pow(state.bombs[i].position.z - state.iaPosition.z, 2);
            if (dist < minDistance)
            {
                minDistance = dist;
            }
        }
        return 1 / minDistance + Mathf.Pow(goal.x - state.iaPosition.x, 2) + Mathf.Pow(goal.z - state.iaPosition.z, 2);
    }

    public float GetLongHeuristic(GameState state, Vector3 goal)
    {
        return 1 / (Mathf.Pow((goal.x - state.iaPosition.x), 2) + Mathf.Pow((goal.z - state.iaPosition.z), 2));
    }

}
