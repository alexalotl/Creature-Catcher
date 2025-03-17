using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private LevelManager level;

    private void Awake()
    {
        level = GetComponent<LevelManager>();

        level.SetupScene();
    }
}
