using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
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

    public GameObject player;

    public bool hasMoved;

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1.0f / moveTime;
        player = GameObject.FindGameObjectWithTag("Player");
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
        if (!mapManager.gameObject.GetComponent<Manager>().playersTurn)
        {
            int horizon = 0;
            int vert = 0;

            int randChance = -1;

            Vector3 playerPos = FindPlayer();

            if (playerPos.x > gameObject.transform.position.x)
            {
                horizon = 1;
            }
            else if (playerPos.x < gameObject.transform.position.x)
            {
                horizon = -1;
            }
            if (playerPos.y > gameObject.transform.position.y)
            {
                vert = 1;
            }
            else if (playerPos.y < gameObject.transform.position.y)
            {
                vert = -1;
            }

            initialPos = gameObject.transform.position;
            tryVector = new Vector3(0, 0, 0);
            if (horizon != 0 && vert != 0)
            {
                randChance = Random.Range(0, 2);

            }
            if (randChance == 0 || ((!(horizon != 0 && vert != 0)) && randChance == -1) && horizon != 0)
            {
                Debug.Log(horizon.ToString() + vert.ToString());

                if (horizon == 1)
                {

                    tryVector = gameObject.transform.position + new Vector3(1, 0, 0);

                }

                else if (horizon == -1)
                {
                    tryVector = gameObject.transform.position + new Vector3(-1, 0, 0);

                    //StartCoroutine(SmoothMovement(tryVector));
                }

                if (horizon != 0)
                {
                    if (CheckMove())
                    {
                        Debug.Log(tryVector);
                        if (tryVector == new Vector3(0, 0, 0))
                        {
                            Debug.Log("Enemy Position: " + gameObject.transform.position + horizon.ToString());

                        }
                        StartCoroutine(SmoothMovement(tryVector));


                        hasMoved = true;
                    }

                    else
                    {
                        Debug.Log(tryVector);

                        Debug.Log("Enemy can't go that way!" + tryVector);

                        hasMoved = true;
                    }
                }
            }

            else if (randChance == 1 || ((!(horizon != 0 && vert != 0)) && randChance == -1) && vert != 0)
            {
                Debug.Log(horizon.ToString() + vert.ToString());

                if (vert == 1)
                {
                    tryVector = gameObject.transform.position + new Vector3(0, 1, 0);
                    //StartCoroutine(SmoothMovement(tryVector));
                }

                else if (vert == -1)
                {
                    tryVector = gameObject.transform.position + new Vector3(0, -1, 0);
                    //StartCoroutine(SmoothMovement(tryVector));
                }
                if (vert != 0)
                {
                    if (CheckMove())
                    {
                        Debug.Log(tryVector);
                        StartCoroutine(SmoothMovement(tryVector));

                        hasMoved = true;
                    }

                    else
                    {
                        Debug.Log(tryVector);

                        Debug.Log("Enemy Can't go that way!" + tryVector);

                        hasMoved = true;
                    }
                }
            }
        }
    }

    IEnumerator SmoothMovement(Vector3 end)
    {
        Debug.Log("Enemy should have moved");
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
            if (mapManager.wallPositions[i] == tryVector)
            {
                return false;
            }
        }

        if(player.transform.position == tryVector)
        {
            return false;
        }

        return true;
    }

    Vector3 FindPlayer()
    {
        Vector3 playerPos = new Vector3(0,0,0);
        playerPos = player.transform.position;
        return playerPos;
    }
}
