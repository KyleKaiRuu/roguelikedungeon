using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public float levelStartDelay = 2.0f;
    public float turnDelay = 0.1f;
    public static Manager instance = null;
    [ReadOnlyField]
    public bool playersTurn = true;

    MapManager mapScript;

    Text levelText;
    GameObject levelImage;
    int level = 0;

    List<GameObject> enemies;
    [ReadOnlyField]
    public bool enemyMoving;

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
        enemies = new List<GameObject>();
        mapScript = GetComponent<MapManager>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        level++;

        InitGame();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void InitGame()
    {
        enemies.Clear();
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Floor: " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        mapScript.SetupScene(level);
    }

    void HideLevelImage()
    {
        levelImage.SetActive(false);
    }

    public void GameOver()
    {
        levelText.text = "You died on floor: " + level;
        levelImage.SetActive(true);
        enabled = false;
    }

    private void Update()
    {
        if (CheckEnemies())
        {
            playersTurn = true;
        }
    }

    bool CheckEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].GetComponent<EnemyMove>().hasMoved)
            {
                return false;
            }
        }
        return true;
    }
}
