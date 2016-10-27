using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;

public class ToolMapScript : EditorWindow
{
    [System.Serializable]
    public class SerializeValue
    {
        public SerializeValue()
        {
        }

        [XmlAttribute()]
        public int seed { get; set; }

        [XmlAttribute()]
        public string nameMap { get; set; }
    }

    private GameState gameState;
    private CollisionManagerScript collisionManagerScript;
    private GameManagerScript gameManager;
    private BombManagerScript[] bombManagers;
    private SerializeValue mySerializeValue;
    private float sizePlaneX;
    private float sizePlaneY;
    private string nameMap = "";
    private string strSizeArene = "";
    private int nbBombs;
    private int seed;
    private BombInfo[] bombs;
    private TriangularMatrixScript<float> bombsDistance;

    public float x0 { get; set; }
    public float z0 { get; set; }
    public float dx { get; set; }
    public float dz { get; set; }

    public void InitToolMapScript()
    {
        nbBombs = 22;
        seed = 0;
        collisionManagerScript = new CollisionManagerScript();
        gameState = new GameState(nbBombs);
        bombManagers = new BombManagerScript[nbBombs];
        bombs = new BombInfo[nbBombs];
    }

    [MenuItem("Tool/Generate Map")]
    public static void ShowWindow()
    {
        EditorWindow myWindow = EditorWindow.GetWindow(typeof(ToolMapScript));
        myWindow.minSize = new Vector2(360f, 350f);
        myWindow.maxSize = new Vector2(370f, 750f);
    }
	
    void OnGUI()
    {
        InitToolMapScript();
        gameManager = new GameManagerScript();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Ajout d'une map test", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Proportion arène : ");
        GUILayoutOption widthTextField = GUILayout.Width(160.0f);
        strSizeArene = GUILayout.TextField(strSizeArene, widthTextField);
        GUILayout.EndHorizontal();

        //sizePlaneX = float.Parse(strSizeArene);
        sizePlaneX = 20f;// sizePlaneX / 2f;

        //sizePlaneY = float.Parse(strSizeArene);
        sizePlaneY = 20f;// sizePlaneY / 2f;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Nom de la map :", EditorStyles.label);

        nameMap = GUILayout.TextField(nameMap, widthTextField);
        GUILayoutOption widthButton = GUILayout.Width(80.0f);

        if (GUILayout.Button("Ajouter", widthButton))
        {
            InitializeGameState();
            Debug.Log("Direction IA : " + gameState.iaDirection);
            Debug.Log("Position IA : " + gameState.iaPosition);
            Debug.Log("minDistBombTo IA : " + gameState.minDistToIA);
            Debug.Log("TimeSinceStart IA : " + gameState.timeSinceStart);
            Debug.Log("BOMBES 1 IA : " + gameState.bombs[0].position);
            Debug.Log("BOMBES 2 IA : " + gameState.bombs[1].position);
            Debug.Log("BOMBES 3 IA : " + gameState.bombs[2].position);
            Debug.Log("BOMBES 4 IA : " + gameState.bombs[3].position);
            Debug.Log("BOMBES 5 IA : " + gameState.bombs[4].position);
            Debug.Log("BOMBES 6 IA : " + gameState.bombs[5].position);
            Debug.Log("BOMBES 7 IA : " + gameState.bombs[6].position);

            Debug.Log("BOMBES DIR 1 IA : " + gameState.bombs[0].direction);
            Debug.Log("BOMBES DIR 2 IA : " + gameState.bombs[1].direction);
            Debug.Log("BOMBES DIR 3 IA : " + gameState.bombs[2].direction);
            Debug.Log("BOMBES DIR 4 IA : " + gameState.bombs[3].direction);
            Debug.Log("BOMBES DIR 5 IA : " + gameState.bombs[4].direction);
            Debug.Log("BOMBES DIR 6 IA : " + gameState.bombs[5].direction);
            Debug.Log("BOMBES DIR 7 IA : " + gameState.bombs[6].direction);
        }
        GUILayout.EndHorizontal();
    }

    public void InitializeGameState()
    {
        if(seed == 0)
        {
            seed = (int)UnityEngine.Random.Range(1f, 1000f);
            UnityEngine.Random.InitState(seed);
        }
        
        gameState.bombs = InitializeBombInfo();
        gameState.iaPosition = new Vector3(-19, .5f, -19);
        gameState.timeSinceStart = Time.time * 1000;

        BombInfo nearestBomb = NearestBombFromIA(gameState.bombs);

        if(Mathf.Abs((nearestBomb.position.x - gameState.iaPosition.x)) > 1 || Mathf.Abs((nearestBomb.position.z - gameState.iaPosition.z)) > 1)
        {
            AddSeedMap();
        }
    }

    public void AddSeedMap()
    {
        //On crée une instance de XmlSerializer dans lequel on lui spécifie le type
        //de l'objet à sérialiser. On utiliser l'opérateur typeof pour cela.
        XmlSerializer serializer = new XmlSerializer(typeof(SerializeValue));

        //Création d'un Stream Writer qui permet d'écrire dans un fichier. On lui spécifie le chemin
        //et si le flux devrait mettre le contenu à la suite de notre document (true) ou s'il devrait
        //l'écraser (false).
        StreamWriter ecrivain = new StreamWriter("Test.xml", true);

        mySerializeValue = new SerializeValue();
        mySerializeValue.nameMap = nameMap;
        mySerializeValue.seed = this.seed;

        //On sérialise en spécifiant le flux d'écriture et l'objet à sérialiser.
        serializer.Serialize(ecrivain, mySerializeValue);
        
        ecrivain.Close();
    }

    public BombInfo[] InitializeBombInfo()
    {
        //Debug.Log("init bomb info");
        nbBombs = bombManagers.Length;
        
        bombsDistance = new TriangularMatrixScript<float>(nbBombs, nbBombs);
        initializeBomb();
        for (var i = 0; i < nbBombs; i++)
        {
            bombs[i] = new BombInfo();
            bombs[i].position.x = bombManagers[i].x0;
            bombs[i].position.z = bombManagers[i].z0;
            bombs[i].direction.x = bombManagers[i].dx;
            bombs[i].direction.y = bombManagers[i].dz;
            bombs[i].delay = -1;
            bombs[i].state = BombState.Normal;
        }

        for (var i = 0; i < nbBombs; i++)
        {
            for (var j = i + 1; j < nbBombs; j++)
            {
                bombsDistance.Set(i, j, Mathf.Sqrt(Mathf.Pow((bombs[i].position.x - bombs[j].position.x), 2)
                                        + Mathf.Pow((bombs[i].position.z - bombs[j].position.z), 2)));
            }
        }

        return bombs;
    }

    public BombInfo NearestBombFromIA(BombInfo[] bombsFromGameState)
    {
        BombInfo result = new BombInfo();
        result.position = new Vector3(1000, 1000, 1000);
        for(var i = 0; i < bombsFromGameState.Length; i++)
        {
            if(result.position.x >= bombsFromGameState[i].position.x && result.position.z >= bombsFromGameState[i].position.y)
            {
                result = bombsFromGameState[i];
            }
        }

        return result;
    }

    public void initializeBomb()
    {
        for(var i = 0; i < bombManagers.Length; i++)
        {
            this.bombManagers[i] = new BombManagerScript();
            this.bombManagers[i].x0 = Random.Range(-sizePlaneX, sizePlaneX);
            this.bombManagers[i].z0 = Random.Range(-sizePlaneY, sizePlaneY);

            int random = Random.Range(1, 9);

            switch (random)
            {
                case 1:
                    this.bombManagers[i].dx = 0.0f;
                    this.bombManagers[i].dz = 1.0f;
                    break;
                case 2:
                    this.bombManagers[i].dx = 0.71f;
                    this.bombManagers[i].dz = 0.71f;
                    break;
                case 3:
                    this.bombManagers[i].dx = 1.0f;
                    this.bombManagers[i].dz = 0.0f;
                    break;
                case 4:
                    this.bombManagers[i].dx = 0.71f;
                    this.bombManagers[i].dz = -0.71f;
                    break;
                case 5:
                    this.bombManagers[i].dx = 0.0f;
                    this.bombManagers[i].dz = -1.0f;
                    break;
                case 6:
                    this.bombManagers[i].dx = -0.71f;
                    this.bombManagers[i].dz = -0.71f;
                    break;
                case 7:
                    this.bombManagers[i].dx = -1.0f;
                    this.bombManagers[i].dz = 0.0f;
                    break;
                case 8:
                    this.bombManagers[i].dx = 0.71f;
                    this.bombManagers[i].dz = -0.71f;
                    break;
            }
        }
    }
}