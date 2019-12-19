using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    //The Game Manager prefab
    public GameObject manager;
    
    void Awake()
    {
        //If there isn't already a manager instance
        if (Manager.instance == null)
        {
            //MAKE ONE
            Instantiate(manager);
        }
    }
}
