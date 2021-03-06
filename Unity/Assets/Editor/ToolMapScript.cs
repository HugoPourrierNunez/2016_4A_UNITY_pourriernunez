﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Xml;

public class ToolMapScript : EditorWindow
{
   
    private GameState gameState;
    private CollisionManagerScript collisionManagerScript;
    private GameManagerScript gameManager;
    private BombManagerScript[] bombManagers;
    private SerializeValue mySerializeValue;
    private float sizePlaneX;
    private float sizePlaneY;
    private string nameMap = "";
    private float sizeArene;
    private string strSizeArene = "";
    private int nbBombs;
    private int seed;
    private BombInfo[] bombs;

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
        myWindow.minSize = new Vector2(360f, 140f);
        myWindow.maxSize = new Vector2(370f, 150f);
    }
	
    void OnGUI()
    {
        InitToolMapScript();
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
        }
        GUILayout.EndHorizontal();
    }

    public void InitializeGameState()
    {
        
        seed = (int)UnityEngine.Random.Range(1f, 1000f);
        UnityEngine.Random.InitState(seed);
        
        
        InitializeBombInfo();
        gameState.iaPosition = new Vector3(-14, .5f, -14);
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
        var emptyNs = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
        XmlSerializer serializer = new XmlSerializer(typeof(SerializeValue));

        //Création d'un Stream Writer qui permet d'écrire dans un fichier. On lui spécifie le chemin
        //et si le flux devrait mettre le contenu à la suite de notre document (true) ou s'il devrait
        //l'écraser (false).
        XmlWriterSettings settingsWriter = new XmlWriterSettings();
        settingsWriter.OmitXmlDeclaration = true;
        StreamWriter seedStream = new StreamWriter("SeedEnable.xml", false);
        var writer = XmlWriter.Create(seedStream, settingsWriter);
        mySerializeValue = new SerializeValue();
        mySerializeValue.nameMap = nameMap;
        mySerializeValue.seed = this.seed;

        //On sérialise en spécifiant le flux d'écriture et l'objet à sérialiser.
        serializer.Serialize(writer, mySerializeValue, emptyNs);

        seedStream.Close();
    }
   

    public void InitializeBombInfo()
    {
        nbBombs = gameState.bombs.Length;
        int random;

        for (var i = 0; i < nbBombs; i++)
        {
            gameState.bombs[i].position.x = UnityEngine.Random.Range(-sizePlaneX, sizePlaneX); ;
            gameState.bombs[i].position.z = UnityEngine.Random.Range(-sizePlaneY, sizePlaneY);

            random = UnityEngine.Random.Range(1, 9);
            var direction = new Vector3(0.0f, 0.0f, 0.0f);
            switch (random)
            {
                case 1:
                    direction.x = 0.0f;
                    direction.z = 1.0f;
                    break;
                case 2:
                    direction.x = 0.71f;
                    direction.z = 0.71f;
                    break;
                case 3:
                    direction.x = 1.0f;
                    direction.z = 0.0f;
                    break;
                case 4:
                    direction.x = 0.71f;
                    direction.z = -0.71f;
                    break;
                case 5:
                    direction.x = 0.0f;
                    direction.z = -1.0f;
                    break;
                case 6:
                    direction.x = -0.71f;
                    direction.z = -0.71f;
                    break;
                case 7:
                    direction.x = -1.0f;
                    direction.z = 0.0f;
                    break;
                case 8:
                    direction.x = 0.71f;
                    direction.z = -0.71f;
                    break;
            }
            gameState.bombs[i].direction = direction;
            gameState.bombs[i].delay = -1;
            gameState.bombs[i].state = BombState.Normal;
        }
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
}
/*
public class SerializeValue
{
    public SerializeValue()
    {
    }

    [XmlElement("Seed")]
    public int seed { get; set; }

    [XmlElement("NameMap")]
    public string nameMap { get; set; }
}*/