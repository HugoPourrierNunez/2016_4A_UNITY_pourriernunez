﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System;

public enum BombState
{
    Normal,
    Explosion,
    BOOM,
    Laser, 
    BZZZ
}

public class SerializeValue
{
    public SerializeValue()
    {
    }

    [XmlElement("Seed")]
    public int seed { get; set; }

    [XmlElement("NameMap")]
    public string nameMap { get; set; }
}


public class GameManagerScript : MonoBehaviour {

    public GameState actualGameState;

    [SerializeField]
    CollisionManagerScript collisionManagerScript;

    [SerializeField]
    MapManagerScript mapManagerScript;

    [SerializeField]
    StateManagerScript stateManagerScript;

    [SerializeField]
    PlayerManagerScript playerManagerScript;

    [SerializeField]
    IAScript iAScript;

    [SerializeField]
    LongTermScript longTermScript;

    [SerializeField]
    AStarGridScript aStarGridScript;

    [SerializeField]
    IAWithAStarScript iAWithAStarScript;

    void OnEnable()
    {
        mapManagerScript.SetGameManagerScript(this);
        collisionManagerScript.SetGameManagerScript(this);
        stateManagerScript.SetGameManagerScript(this);
        playerManagerScript.SetGameManagerScript(this);

        if (iAScript != null)
        {
            iAScript.SetGameManagerScript(this);
        }

        if (iAWithAStarScript != null)
        {
            iAWithAStarScript.SetGameManagerScript(this);
        }

        if (aStarGridScript != null)
        {
            aStarGridScript.SetGameManagerScript(this);
        }

        InitializeGameState();

        //////////////////////////////////////////////////
        //              Test multi-threading            //
        //////////////////////////////////////////////////



    }

    public List<int> RetrieveAllSeedMap()
    {
       List<int> allSeed = new List<int>();


        FileStream fs = new FileStream("SeedEnable.xml", FileMode.Open);
       
        XmlReader reader = XmlReader.Create(fs);

        XmlSerializer deserializer = new XmlSerializer(typeof(SerializeValue));

        var result = (SerializeValue)deserializer.Deserialize(reader);

        /*
        for (var i = 0; i < result.Count; i++)
        {
            allSeed.Add(result[i].seed);
        }*/
        
        allSeed.Add(result.seed);
        fs.Close();
        
        return allSeed;
    }

    public void InitializeGameState()
    {
        List<int> allSeed = RetrieveAllSeedMap();
 
        int seedChoosen = UnityEngine.Random.Range(0, allSeed.Count - 1);

        UnityEngine.Random.InitState(allSeed[seedChoosen]);
        actualGameState = new GameState(CollisionManagerScript.getBombManagers().Length);

        actualGameState.iaPosition = new Vector3(mapManagerScript.GetStartTransform().position.x, .5f, mapManagerScript.GetStartTransform().position.z);
        collisionManagerScript.InitializeBombInfo(actualGameState);
        actualGameState.timeSinceStart = Time.time * 1000;
        if(aStarGridScript != null)
		{
			aStarGridScript.InitializeGridDimensions();
		}
    }

    public CollisionManagerScript CollisionManagerScript
    {
        get
        {
            return collisionManagerScript;
        }
    }

    public MapManagerScript MapManagerScript
    {
        get
        {
            return mapManagerScript;
        }
    }

    public StateManagerScript StateManagerScript
    {
        get
        {
            return stateManagerScript;
        }
    }

    public GameState ActualGameState
    {
        get
        {
            return actualGameState;
        }

        set
        {
            actualGameState = value;
        }
    }

    public IAScript IAScript
    {
        get
        {
            return iAScript;
        }

    }

    public IAWithAStarScript IAWithAStarScript
    {
        get
        {
            return iAWithAStarScript;
        }

    }
}
