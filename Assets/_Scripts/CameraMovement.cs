using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lerpSpeed = 0.5f;


    void Start()
    {
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        Vector3 newPosition = player.transform.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, newPosition, lerpSpeed * Time.deltaTime);
        transform.position = smoothPosition;
    }
}
