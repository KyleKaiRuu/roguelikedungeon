using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 50;

    GameObject player;
    MapManager mapManager;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mapManager = GameObject.Find("Manager(Clone)").GetComponent<MapManager>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            mapManager.enemies.Remove(gameObject);

            //player.GetComponent<PlayerMove>().nearbyEnemies.Remove(gameObject);
            //player.GetComponent<PlayerMove>().enemiesDirections.Remove(gameObject.transform.position);
            Destroy(gameObject);
        }
    }
}
