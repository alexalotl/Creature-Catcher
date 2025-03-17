using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class Creature
{
    [SerializeField]
    public GameObject creature;
    public int weight;
}

public class LevelManager : MonoBehaviour
{
    public bool isGarden = false;
    public float minK, maxK;
    public float minM, maxM;
    public float minN, maxN;
    public float minC, maxC;
    public int levelHeight = 128;
    public int levelWidth = 100;
    public Tilemap dirtMap;
    public RuleTile dirtTile;
    public GameObject player;
    public int maxCreatures = 10;
    public CreatureLookup lookup;
    [SerializeField]
    public Creature[] creatures;

    public CameraRegion camRegion;

    private int leftWorldBuffer = 3;
    private int rightWorldBuffer = 3;
    
    private List<Vector3Int> tileLocations = new List<Vector3Int>();

    private List<Vector3Int> spawnLocations = new List<Vector3Int>();

    private int totalWeight = 0;

    public struct CreatureStats
    {
    }
    struct SinFunction
    {
        // Function is written as f(x, y) = ksin(mx+ny+c)
        public float k;
        public float m;
        public float n;
        public float c;

        public SinFunction(float k, float m, float n, float c)
        {
            this.k = k;
            this.m = m;
            this.n = n;
            this.c = c;
        }
    }

    SinFunction CreateSinFunction()
    {
        float k = Random.Range(minK, maxK); // Peak height variance e.g 0.01f, 0.09f
        float m = Random.Range(minM, maxM); // Peak density e.g 0f, 2f
        float n = Random.Range(minN, maxN); // Peak slant e.g 0f, 0.2f
        float c = Random.Range(minC, maxC); // Peak offset e.g 1.00f, 10.00f
        SinFunction func = new SinFunction(k, m, n, c);
        return func;
    }
    void CreateLayout()
    {
        tileLocations.Clear();

        int sinIterations = 6;
        List<SinFunction> sinFunctions = new List<SinFunction>();
        for (int i = 0; i < sinIterations; i++)
        {
            sinFunctions.Add(CreateSinFunction());
        }

        for (int x = 1; x < levelWidth; x++)
        {
            for (int y = 1; y < levelHeight; y++)
            {
                if (x <= 3 || x >= levelWidth - 3 || y <= 3)
                {
                    tileLocations.Add(new Vector3Int(x, y, 0));
                }
                else
                {
                    float sinSum = 0;
                    foreach (SinFunction func in sinFunctions)
                    {
                        sinSum += func.k * Mathf.Sin(func.m * (x - levelWidth / 2) + func.n * (y - levelHeight / 2) + func.c);
                    }
                    if (0.1 * (y - 8) <= sinSum)
                    {
                        tileLocations.Add(new Vector3Int(x, y, 0));
                    }
                }
            }
        }
    }

    private void FindSpawnLocations()
    {
        spawnLocations.Clear();
        foreach (Vector3Int tilePos in tileLocations){
            if (tilePos.x > leftWorldBuffer && tilePos.x < levelWidth - rightWorldBuffer && !tileLocations.Contains(tilePos + new Vector3Int(0, 1, 0)) && !tileLocations.Contains(tilePos + new Vector3Int(0, 2, 0)))
            {
                spawnLocations.Add(tilePos + new Vector3Int(0, 1, 0));
            }
        }
    }

    List<CreatureData> LoadData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(Application.persistentDataPath
                   + "/CreatureData.dat"))
        {
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/CreatureData.dat", FileMode.Open);
            List<CreatureData> creatureList = (List<CreatureData>)bf.Deserialize(file);
            foreach (CreatureData c in creatureList)
            {
                print(c.type + c.dateCaught);
            }
            file.Close();
            return creatureList;
        }
        else
        {
            return new List<CreatureData>();
        }
    }

    private int getSpawnIndex()
    {
        int spawnIndex = Random.Range(0, spawnLocations.Count);
        return spawnIndex;
    }

    public void SetupScene()
    {
        List<CreatureData> creatureList = new List<CreatureData>();
        if (isGarden){
            creatureList = LoadData();
            levelWidth = (creatureList.Count <= 18 ? 32 : creatureList.Count + 16);
        }

        CreateLayout();

        FindSpawnLocations();

        foreach (Vector3Int tilePos in tileLocations)
        {
            dirtMap.SetTile(tilePos, dirtTile);
        }

        int spawnIndex = getSpawnIndex();
        Vector3Int spawn = spawnLocations[spawnIndex];
        spawnLocations.RemoveAt(spawnIndex);
        player.transform.position = spawn + new Vector3(0.5f, 2.0f, 0.0f);

        foreach (Creature c in creatures)
        {
            totalWeight += c.weight;
        }

        if (isGarden)
        {
            foreach (CreatureData cData in creatureList)
            {
                int creatureIndex = System.Array.FindIndex(lookup.lookupTable, x => x.type == cData.type);
                print(creatureIndex);
                GameObject creaturePrefab = lookup.lookupTable[creatureIndex].prefab;
                spawnIndex = getSpawnIndex();
                spawn = spawnLocations[spawnIndex];
                spawnLocations.RemoveAt(spawnIndex);

                GameObject creature = Instantiate(creaturePrefab, spawn + new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity);
                CreatureManager creatureScript = creature.GetComponent<CreatureManager>();
                creatureScript.DoB = cData.dateCaught;
                creatureScript.isCaught = true;
            }
        }
        else {
            for (int i = 0; i <= maxCreatures; i++)
            {
                print(totalWeight);
                spawnIndex = getSpawnIndex();
                spawn = spawnLocations[spawnIndex];
                spawnLocations.RemoveAt(spawnIndex);
                int creatureSelection = Random.Range(0, totalWeight + 1);
                foreach (Creature c in creatures)
                {
                    if (c.weight >= creatureSelection)
                    {
                        Instantiate(c.creature, spawn + new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity);
                        break;
                    }
                    else
                    {
                        creatureSelection -= c.weight;
                    }
                }
            }
        }

        camRegion.xBounds.y = levelWidth - 10;

        camRegion.yBounds.y = levelHeight;
    }


    public void DestroyBlock(Vector3Int location)
    {
        dirtMap.SetTile(location, null);
    }
}
