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

    bool settingMap;

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
        settingMap = true;
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
        settingMap = false;
    }

    public void GameOver()
    {
        levelText.text = "You died on floor: " + level;
        levelImage.SetActive(true);
        enabled = false;
    }

    private void Update()
    {
        //Move this into PlayerMove/EnemyMove then delete Update Function
        if (playersTurn || enemyMoving)
        {
            return;
        }
    }

    public void AddEnemyToList()
    {
        //Add enemies to a list when they get instantiated, move to MapManager
        //enemies.Add();
    }
    //Move into own script, leaving just in case I need something from it
    IEnumerator MoveEnemies()
    {
        enemyMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            //enemies[i].MoveEnemy();

            //yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;
        enemyMoving = false;
    }
}
