using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static DontDestroyOnLoad player;


    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (player == null)
            player = this;
        else
            Destroy(gameObject);
    }

}
