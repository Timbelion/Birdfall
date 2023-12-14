using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeTrees : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float offset;


    void Update()
    {
        transform.position = new Vector3(player.transform.position.x + offset, transform.position.y, transform.position.z);
    }
}
