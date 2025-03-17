using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CreatureLookupScriptableObject", order = 1)]
public class CreatureLookup : ScriptableObject
{

    [System.Serializable]
    public struct Creature
    {
        public string type;
        public GameObject prefab;
    }

    public Creature[] lookupTable;
    
}
