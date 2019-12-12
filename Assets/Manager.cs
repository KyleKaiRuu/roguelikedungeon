using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance = null;
    MapManager mapScript;
    public int level = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        mapScript = GetComponent<MapManager>();
        InitGame();
    }

    void InitGame()
    {
        mapScript.SetupScene(level);
    }
}
