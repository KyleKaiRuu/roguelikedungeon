﻿using System.Collections;
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
    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
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

                    }
                    else if (horizon == -1)
                    {
                        tryVector = gameObject.transform.position + new Vector3(-1, 0, 0);
                        //StartCoroutine(SmoothMovement(tryVector));
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
                        //StartCoroutine(SmoothMovement(tryVector));
                    }
                    else if (vert == -1)
                    {
                        tryVector = gameObject.transform.position + new Vector3(0, -1, 0);
                        //StartCoroutine(SmoothMovement(tryVector));
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
                        mapManager.gameObject.GetComponent<Manager>().playersTurn = false;

                    }
                }
            }
        }
    }

    IEnumerator SmoothMovement(Vector3 end)
    {
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
}
