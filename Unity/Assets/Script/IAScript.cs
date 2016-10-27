using UnityEngine;
using System.Collections;

public class IAScript : MonoBehaviour {

    [SerializeField]
    int analyseDepth = 3;

    GameManagerScript gameManagerScript;

    GameState[] gameStates;

    private int sizeGameState;


    public static Vector3[] Direction = new Vector3[8]
    {
        new Vector3(0.0f,0.0f, 1.0f),
        new Vector3(0.71f,0.0f, 0.71f),
        new Vector3(1.0f,0.0f, 0.0f),
        new Vector3(-0.71f,0.0f, 0.71f),
        new Vector3(-1.0f,0.0f, 0.0f),
        new Vector3(-0.71f,0.0f, -0.71f),
        new Vector3(0.0f,0.0f, -1.0f),
        new Vector3(0.71f,0.0f, -0.71f)
    };

    public void setGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }


    public Vector3 getNextDirectionIA()
    {
        //tree.val = -1;
        //generateTree(tree,gameManagerScript.ActualGameState, analyseDepth);

        for(var i = 0; i<sizeGameState;i++)
        {
            gameStates[i].score = float.MaxValue;
        }

        
        for(var i=0;i<8;i++)
        {
            explore(gameManagerScript.ActualGameState, 1, i);
        }
        /*for (var i = 0; i < sizeGameState; i++)
        {
            if(gameStates[i].score != float.MaxValue)Debug.Log(gameStates[i].score);
        }*/

        return analyse();
    }

    public Vector3 analyse()
    {
        var minScore = float.MaxValue;
        var minVector = Vector3.zero;

        for(var i=(int) Mathf.Pow(8,analyseDepth-1); i<sizeGameState;i++)
        {
            if(gameStates[i].score<minScore)
            {
                minScore = gameStates[i].score;
                minVector = gameStates[i].originalDirection;
            }
        }
        //Debug.Log(minScore);
        return minVector;
    }

    public void explore(GameState gs, int depth, int index)
    {
        if (depth > analyseDepth)
            return;

        for(var i = 0; i < 8; i++)
        {
            if (depth == 1)
            {
                gameStates[index].originalDirection = Direction[i];
            }
            else
            {
                gameStates[index].originalDirection = gs.originalDirection;
            }

            gameManagerScript.CollisionManagerScript.FillNextGameState(gs, gameStates[index], Direction[i]);

            //Quitte si collision
            if (gameStates[index].score == float.MaxValue)
                return;

            var nb = index - (int)Mathf.Pow(8, depth - 1) + 1;

            explore(gameStates[index], depth + 1, (int)Mathf.Pow(8, depth-1) + i - 1);
            //explore(gameStates[index],depth+1, (int)Mathf.Pow(8, depth) + 7 * nb + i);
        }
    }


    void Start()
    {
        sizeGameState = (int)Mathf.Pow(8, analyseDepth);
        var nbBombs = gameManagerScript.CollisionManagerScript.getBombManagers().Length;
        gameStates = new GameState[sizeGameState];
        for(var i=0;i<sizeGameState;i++)
        {
            gameStates[i] = new GameState(nbBombs);
        }
    }
}
