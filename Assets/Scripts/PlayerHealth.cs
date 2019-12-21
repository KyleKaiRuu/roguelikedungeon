﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int defaultHealth;

    public int health; 
    public float timer;
    public float delay;

    public Canvas gameOverCanvas;

    Manager manager;

    private void Awake()
    {
        manager = GameObject.Find("Manager(Clone)").GetComponent<Manager>();
        
        health = defaultHealth;
    }
    private void Update()
    {
        if (gameOverCanvas == null)
        {
            gameOverCanvas = GameObject.Find("GameOverCanvas(Clone)").GetComponent<Canvas>();
        }
        if (manager == null)
        {
            Debug.Log("NULLLLLLLLL");
        }
        if (health <= 0)
        {
            gameOverCanvas.enabled = true;

            timer += Time.deltaTime;

            if (timer >= delay)
            {
                manager.level = 0;
                health = defaultHealth;
                SceneManager.LoadScene(0);
            }
        }
    }
}
