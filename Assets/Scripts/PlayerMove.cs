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

    [ReadOnlyField]
    public bool enemyNear = false;

    [ReadOnlyField]
    public List<GameObject> nearbyEnemies = new List<GameObject>();
    [ReadOnlyField]
    public List<Vector3> enemiesDirections = new List<Vector3>();
    
    [ReadOnlyField]
    public int direction = 0;

    [ReadOnlyField]
    public Vector3 directionVector = new Vector3(0, -1, 0);

    public int damage = 10;

    const int frontAni = 0;
    const int leftAni = 1;
    const int rightAni = 2;
    const int backAni = 3;

    int currentAni = frontAni;
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

        CheckForEnemies();

        if (mapManager.gameObject.GetComponent<Manager>().playersTurn || mapManager.enemies.Count == 0)
        {

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
                        direction = rightAni;
                        directionVector = new Vector3(1, 0, 0);
                    }
                    else if (horizon == -1)
                    {
                        tryVector = gameObject.transform.position + new Vector3(-1, 0, 0);
                        direction = leftAni;
                        directionVector = new Vector3(-1, 0, 0);
                    }

                    if (CheckMove())
                    {
                        StartCoroutine(SmoothMovement(tryVector));
                        timer = 0;
                        foreach (GameObject enemy in mapManager.enemies)
                        {
                            enemy.GetComponent<EnemyMove>().hasMoved = false;
                        }
                        mapManager.gameObject.GetComponent<Manager>().playersTurn = false;

                    }
                    else
                    {
                        ChangeAnimationState(direction);
                        timer = 0;
                    }
                }

                if (vert != 0)
                {
                    if (vert == 1)
                    {
                        tryVector = gameObject.transform.position + new Vector3(0, 1, 0);
                        direction = backAni;
                        directionVector = new Vector3(0, 1, 0);
                    }
                    else if (vert == -1)
                    {
                        tryVector = gameObject.transform.position + new Vector3(0, -1, 0);
                        direction = frontAni;
                        directionVector = new Vector3(0, -1, 0);
                    }

                    if (CheckMove())
                    {
                        StartCoroutine(SmoothMovement(tryVector));
                        timer = 0;
                        foreach (GameObject enemy in mapManager.enemies)
                        {
                            enemy.GetComponent<EnemyMove>().hasMoved = false;
                        }
                        mapManager.gameObject.GetComponent<Manager>().playersTurn = false;
                    }
                    else
                    {
                        ChangeAnimationState(direction);
                        
                        timer = 0;
                    }
                }

                if (enemyNear)
                {
                    if (Input.GetAxis("Fire1") != 0)
                    {
                        for (int i = 0; i < enemiesDirections.Count; i++)
                        {
                            Debug.Log(directionVector + " " + enemiesDirections[i]);
                            if (directionVector == enemiesDirections[i])
                            {
                                Debug.Log(directionVector);
                                nearbyEnemies[i].GetComponent<EnemyHealth>().health -= damage;
                                timer = 0;
                                foreach (GameObject enemy in mapManager.enemies)
                                {
                                    enemy.GetComponent<EnemyMove>().hasMoved = false;
                                }
                                mapManager.gameObject.GetComponent<Manager>().playersTurn = false;
                                
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator SmoothMovement(Vector3 end)
    {
        ChangeAnimationState(direction);

        nearbyEnemies.Clear();
        enemiesDirections.Clear();
        enemyNear = false;

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
        bool anyEnemyNear = false;
        int i = 0;
        foreach(GameObject enemy in mapManager.enemies)
        {
            
            if (enemy.transform.position == gameObject.transform.position + new Vector3(1, 0, 0) ||
                enemy.transform.position == gameObject.transform.position + new Vector3(-1, 0, 0) ||
                enemy.transform.position == gameObject.transform.position + new Vector3(0, 1, 0) ||
                enemy.transform.position == gameObject.transform.position + new Vector3(0, -1, 0))
            {
                enemyNear = true;
                anyEnemyNear = true;
                if (nearbyEnemies.Find(o => o == mapManager.enemies[i]) == null)
                {
                    nearbyEnemies.Add(mapManager.enemies[i]);
                    enemiesDirections.Add(GetDirectionOfEnemy(mapManager.enemies[i]));
                }
            }

            else
            {
                if (!anyEnemyNear)
                {
                    enemyNear = false;
                }
            }

            i++;
        }
    }

    void ChangeAnimationState(int state)
    {
        animator.SetInteger("Direction", direction);
        currentAni = direction;
    }
}
