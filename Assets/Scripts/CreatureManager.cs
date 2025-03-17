using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
class CreatureData
{
    public string type;
    public string dateCaught;
}

public class CreatureManager : MonoBehaviour
{

    public string type;
    public string DoB;
    public bool isCaught = false;
    private AudioSource audioSource;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        int n = Random.Range(0, 500);
        if (n == 1)
        {
            sprite.flipX = !sprite.flipX;
        }
    }

    protected virtual void OnCollect()
    {
        CreatureData newCreature = new CreatureData();
        newCreature.type = type;
        print(System.DateTime.Now.ToString());
        newCreature.dateCaught = System.DateTime.Now.ToString();

        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath
                   + "/CreatureData.dat"))
        {
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/CreatureData.dat", FileMode.Open);
            List<CreatureData> creatureList = (List<CreatureData>)bf.Deserialize(file);
            creatureList.Add(newCreature);
            foreach (CreatureData c in creatureList)
            {
                print(c.type + c.dateCaught);
            }

            file.Close();
            // This is so fucking bad but it's 4am and I just need something that works right now
            File.Delete(Application.persistentDataPath
                       + "/CreatureData.dat");

            file = File.Create(Application.persistentDataPath
                 + "/CreatureData.dat");
            bf.Serialize(file, creatureList);
            file.Close();
        }
        else
        {
            FileStream file = File.Create(Application.persistentDataPath
                 + "/CreatureData.dat");
            List<CreatureData> creatureList = new List<CreatureData>();
            creatureList.Add(newCreature);
            bf.Serialize(file, creatureList);
            file.Close();
        }
    }

    // General rules for collecting
    public bool Collect()
    {
        if (!isCaught)
        {
            OnCollect();
            gameObject.SetActive(false);
            return true;
        }
        return false;
    }
}
