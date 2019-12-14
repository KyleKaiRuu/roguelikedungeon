using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            maximum = max;
            minimum = min;
        }
    }

    public int columns = 20;
    public int rows = 20;
    public Count wallCount = new Count(5, 9);
    public Count itemCount = new Count(1, 5);

    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] itemTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] player;

    private Transform mapHolder;
    [ReadOnlyField]
    public List<Vector3> gridPositions = new List<Vector3>();

    [ReadOnlyField]
    public List<Vector3> wallPositions = new List<Vector3>();
    [ReadOnlyField]
    public List<GameObject> enemies = new List<GameObject>();

    void InitializeList()
    {
        gridPositions.Clear();
        for (int i = 1; i < columns - 1; i++)
        {
            for (int j = 1; j < rows - 1; j++)
            {
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }
    }

    void MapSetup()
    {
        mapHolder = new GameObject("Map").transform;
        wallPositions.Clear();
        enemies.Clear();
        for (int i = -1; i < columns + 1; i++)
        {
            for (int j = -1; j < rows + 1; j++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (i == -1 || i == columns || j == -1 || j == rows)
                {

                    GameObject outerFloor = Instantiate(toInstantiate, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                    outerFloor.transform.SetParent(mapHolder);

                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    if (toInstantiate.tag == "Wall")
                    {
                        wallPositions.Add(new Vector3(i, j, 0));
                    }

                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(mapHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randIndex = Random.Range(0, gridPositions.Count);
        Vector3 randPos = gridPositions[randIndex];
        gridPositions.RemoveAt(randIndex);
        return randPos;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randPos = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            if (tileChoice.tag == "Wall")
            {
                wallPositions.Add(randPos);
            }

            GameObject instance = Instantiate(tileChoice, randPos, Quaternion.identity);

            if (instance.tag == "Enemy")
            {
                enemies.Add(instance);
            }
        }
    }

    void LayoutExit(GameObject gameObject)
    {
        Vector3 randPos = RandomPosition();
        Instantiate(gameObject, randPos, Quaternion.identity);
    }

    public void SetupScene(int level)
    {
        MapSetup();
        InitializeList();
        LayoutObjectAtRandom(player, 1, 1);
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(itemTiles, itemCount.minimum, itemCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        LayoutExit(exit);
    }
}
