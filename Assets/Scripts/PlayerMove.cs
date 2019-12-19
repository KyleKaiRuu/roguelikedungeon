using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    //[ReadOnlyField]
    //public bool isDelayed;

    public float delay = 1.0f;

    [ReadOnlyField]
    public float timer = 0;

    public float moveTime = 0.1f;

    Rigidbody2D rgbd;
    float inverseMoveTime;

    Vector3 initialPos;
    Vector3 tryVector;
    [ReadOnlyField]
    public MapManager mapManager;

    public float restartDelay = 0.5f;

    Animator animator;

    bool enemyNear;

    [ReadOnlyField]
    public List<GameObject> nearbyEnemies = new List<GameObject>();
    [ReadOnlyField]
    public List<Vector3> enemiesDirections = new List<Vector3>();

    bool checkedForEnemy;

    public int direction = 0;
    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inverseMoveTime = 1.0f / moveTime;
    }

    void Update()
    {
        if (mapManager == null)
        {
            if (GameObject.Find("Manager(Clone)").GetComponent<MapManager>() != null)
            {
                mapManager = GameObject.Find("Manager(Clone)").GetComponent<MapManager>();
            }
        }
        if (mapManager.gameObject.GetComponent<Manager>().playersTurn || mapManager.enemies.Count == 0)
        {
            CheckForEnemies();

            int horizon = 0;
            int vert = 0;

            horizon = (int)(Input.GetAxisRaw("Horizontal"));

            vert = (int)(Input.GetAxisRaw("Vertical"));
            if (timer < delay)
            {
                timer += Time.deltaTime;
            }

            if (timer >= delay)
            {
                initialPos = gameObject.transform.position;
                tryVector = new Vector3(0, 0, 0);
                if (horizon != 0)
                {
                    if (horizon == 1)
                    {
                        tryVector = gameObject.transform.position + new Vector3(1, 0, 0);
                        direction = 2;
                    }
                    else if (horizon == -1)
                    {
                        tryVector = gameObject.transform.position + new Vector3(-1, 0, 0);
                        direction = 1;
                    }

                    if (CheckMove())
                    {
                        StartCoroutine(SmoothMovement(tryVector));
                        timer = 0;

                        mapManager.gameObject.GetComponent<Manager>().playersTurn = false;

                    }
                    else
                    {
                        Debug.Log("Can't go that way!");
                        timer = 0;
                    }
                }

                if (vert != 0)
                {
                    if (vert == 1)
                    {
                        tryVector = gameObject.transform.position + new Vector3(0, 1, 0);
                        direction = 3;
                    }
                    else if (vert == -1)
                    {
                        tryVector = gameObject.transform.position + new Vector3(0, -1, 0);
                        direction = 0;
                    }

                    if (CheckMove())
                    {
                        StartCoroutine(SmoothMovement(tryVector));
                        timer = 0;

                        mapManager.gameObject.GetComponent<Manager>().playersTurn = false;
                    }
                    else
                    {

                        Debug.Log("Can't go that way!");
                        timer = 0;
                    }
                }
            }
        }
    }

    IEnumerator SmoothMovement(Vector3 end)
    {
        animator.SetInteger("Direction", direction);

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(rgbd.position, end, inverseMoveTime * Time.deltaTime);
            rgbd.MovePosition(newPos);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }
    }

    bool CheckMove()
    {
        for (int i = 0; i < mapManager.wallPositions.Count; i++)
        {
            if (mapManager.wallPositions[i] == tryVector )
            {
                return false;
            }
        }

        for (int i = 0; i < mapManager.enemies.Count; i++)
        {
            if (mapManager.enemies[i].transform.position == tryVector)
            {
                return false;
            }
        }
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Invoke("Restart", restartDelay);
    }

    void Restart()
    {
        SceneManager.LoadScene(0);
    }

    Vector3 GetDirectionOfEnemy(GameObject nearbyEnemy)
    {
        if (nearbyEnemy.transform.position == gameObject.transform.position + new Vector3(1, 0, 0))
        {
            return new Vector3(1, 0, 0);
        }
        else if (nearbyEnemy.transform.position == gameObject.transform.position + new Vector3(-1, 0, 0))
        {
            return new Vector3(-1, 0, 0);
        }
        else if (nearbyEnemy.transform.position == gameObject.transform.position + new Vector3(0, 1, 0))
        {
            return new Vector3(0, 1, 0);
        }
        else if (nearbyEnemy.transform.position == gameObject.transform.position + new Vector3(0, -1, 0))
        {
            return new Vector3(0, -1, 0);
        }

        return new Vector3(0,0,0);
    }

    void CheckForEnemies()
    {
        for (int i = 0; i < mapManager.enemies.Count; i++)
        {
            if (mapManager.enemies[i].transform.position == gameObject.transform.position + new Vector3(1, 0, 0) ||
                mapManager.enemies[i].transform.position == gameObject.transform.position + new Vector3(-1, 0, 0) ||
                mapManager.enemies[i].transform.position == gameObject.transform.position + new Vector3(0, 1, 0) ||
                mapManager.enemies[i].transform.position == gameObject.transform.position + new Vector3(0, -1, 0))
            {
                enemyNear = true;
                if (nearbyEnemies.Find(o => o == mapManager.enemies[i]) == null)
                {
                    nearbyEnemies.Add(mapManager.enemies[i]);
                    enemiesDirections.Add(GetDirectionOfEnemy(mapManager.enemies[i]));
                }
            }
            else
            {
                enemyNear = false;
                nearbyEnemies.Clear();
                enemiesDirections.Clear();
            }
        }
    }
}
