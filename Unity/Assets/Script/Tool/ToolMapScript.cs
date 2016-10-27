using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;

public class ToolMapScript : EditorWindow
{
    [System.Serializable]
    public struct SerializeValue
    {
        public List<Seed> listSeed;
    }

    [System.Serializable]
    public class Personne
    {
        [XmlAttribute()]
        public string Nom { get; set; }

        [XmlAttribute()]
        public string Prenom { get; set; }

        [XmlAttribute()]
        public int Age { get; set; }
    }

    private GameState gameState;
    private CollisionManagerScript collisionManagerScript;
    private IAManagerScript iaManagerScript;
    private GameManagerScript gameManager;
    private BombManagerScript[] bombManagers;
    private SerializeValue mySerializeValue;
    private string nameMap = "";
    private int nbBombs;
    private BombInfo[] bombs;
    private TriangularMatrixScript<float> bombsDistance;

    public void InitToolMapScript()
    {
        collisionManagerScript = new CollisionManagerScript();
        iaManagerScript = new IAManagerScript();
        gameState = new GameState();
        bombManagers = new BombManagerScript[22];
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
        mySerializeValue.listSeed = new List<Seed>();
        gameManager = new GameManagerScript();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Ajout d'une map test", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Nom de la map :", EditorStyles.label);
        GUILayoutOption widthTextField = GUILayout.Width(160.0f);
        nameMap = GUILayout.TextField(nameMap, widthTextField);
        GUILayoutOption widthButton = GUILayout.Width(80.0f);

        if (GUILayout.Button("Ajouter", widthButton))
        {
            InitializeGameState();
            Debug.Log("Direction IA : " + gameState.iaDirection);
            Debug.Log("Position IA : " + gameState.iaPosition);
            Debug.Log("minDistBombTo IA : " + gameState.minDistToIA);
            Debug.Log("TimeSinceStart IA : " + gameState.timeSinceStart);
            Debug.Log("BOMBES IA : " + gameState.bombs);
            //On crée une instance de XmlSerializer dans lequel on lui spécifie le type
            //de l'objet à sérialiser. On utiliser l'opérateur typeof pour cela.
            XmlSerializer serializer = new XmlSerializer(typeof(GameState));

            //Création d'un Stream Writer qui permet d'écrire dans un fichier. On lui spécifie le chemin
            //et si le flux devrait mettre le contenu à la suite de notre document (true) ou s'il devrait
            //l'écraser (false).
            StreamWriter ecrivain = new StreamWriter("Test.xml", false);

            //On sérialise en spécifiant le flux d'écriture et l'objet à sérialiser.
            serializer.Serialize(ecrivain, gameState);

            //IMPORTANT : On ferme le flux en tous temps !!!
            ecrivain.Close();
        }
        GUILayout.EndHorizontal();
    }

    public void InitializeGameState()
    {
        gameState.bombs = InitializeBombInfo();
        gameState.timeSinceStart = Time.time * 1000;
        iaManagerScript.getIaTransform().position = new Vector3(-19, .5f, -19);
    }

    public BombInfo[] InitializeBombInfo()
    {
        //Debug.Log("init bomb info");
        nbBombs = bombManagers.Length;

        bombs = new BombInfo[nbBombs];
        bombsDistance = new TriangularMatrixScript<float>(nbBombs, nbBombs);

        for (var i = 0; i < nbBombs; i++)
        {
           
            bombManagers[i].initializeBomb(gameManager.MapManagerScript.getPlaneTransform());
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

    public void initializeBomb(Transform plane)
    {
        /*this.x0 = transform.position.x;
        this.z0 = transform.position.z;*/

        this.x0 = Random.Range(plane.position.x - plane.localScale.x * 5 + bombGO.transform.localScale.x / 2, plane.position.x + plane.localScale.x * 5 - bombGO.transform.localScale.x / 2);
        this.z0 = Random.Range(plane.position.z - plane.localScale.z * 5 + bombGO.transform.localScale.z / 2, plane.position.z + plane.localScale.z * 5 - bombGO.transform.localScale.z / 2);

        int random = Random.Range(1, 9);

        switch (random)
        {
            case 1:
                this.dx = 0.0f;
                this.dz = 1.0f;
                break;
            case 2:
                this.dx = 0.71f;
                this.dz = 0.71f;
                break;
            case 3:
                this.dx = 1.0f;
                this.dz = 0.0f;
                break;
            case 4:
                this.dx = 0.71f;
                this.dz = -0.71f;
                break;
            case 5:
                this.dx = 0.0f;
                this.dz = -1.0f;
                break;
            case 6:
                this.dx = -0.71f;
                this.dz = -0.71f;
                break;
            case 7:
                this.dx = -1.0f;
                this.dz = 0.0f;
                break;
            case 8:
                this.dx = 0.71f;
                this.dz = -0.71f;
                break;
        }
    }
}

public class Seed
{

}
