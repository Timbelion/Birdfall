using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [SerializeField] private float chunkNumber = 3.0f;
    private GameObject player;
    private float offsetX = 19.44995f, offsetY = 0;
    [SerializeField] private GameObject tree1, tree2;
    [SerializeField] private GameObject cornerLeftBot, cornerLeftTop, cornerRightTop, cornerRightBot;
    [SerializeField] private GameObject coin;
    [SerializeField] private bool initializeTerrain = false;
    [SerializeField] private GameManager gameManager;

    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        if (initializeTerrain)
            StartCoroutine(InitializeTerrain());
    }

    void Update()
    {
        if (player.transform.position.x > transform.position.x + offsetX)
        {
            Instantiate(Resources.Load("Terrain"), new Vector3(chunkNumber * offsetX + transform.position.x, chunkNumber * offsetY + transform.position.y, 0), transform.rotation);
            Destroy(gameObject);
        }
    }

    private IEnumerator InitializeTerrain()
    {
        RaycastHit hit;
        for (int i = 0; i < 3; i++)
        {
            float xCor = Random.Range(cornerLeftBot.transform.position.x, cornerLeftTop.transform.position.x);
            float zCor = Random.Range(cornerLeftBot.transform.position.z, cornerRightBot.transform.position.z);

            if (Physics.Raycast(new Vector3(xCor, 10, zCor), -Vector3.up, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Terrain")
                {
                    var instance = Instantiate(coin, new Vector3(xCor, 1, zCor), Quaternion.identity);
                    instance.transform.parent = transform;
                }
            }
        }

        for (int i = 0; i < gameManager.density; i++)
        {
            float xCor = Random.Range(cornerLeftBot.transform.position.x, cornerLeftTop.transform.position.x);
            float zCor = Random.Range(cornerLeftBot.transform.position.z, cornerRightBot.transform.position.z);

            if (Physics.Raycast(new Vector3(xCor, 10, zCor), -Vector3.up, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Terrain")
                {
                    if (Random.Range(1, 100) < 50)
                    {
                        var instance = Instantiate(tree1, new Vector3(xCor, hit.point.y, zCor), Quaternion.identity);
                        float scale = Random.Range(0.6f, 1.4f);
                        float currentScale = instance.transform.localScale.x;
                        instance.transform.localScale = new Vector3(scale * currentScale, scale * currentScale, scale * currentScale);
                        instance.transform.parent = transform;
                    }
                    else
                    {
                        var instance = Instantiate(tree2, new Vector3(xCor, hit.point.y, zCor), Quaternion.identity);
                        float scale = Random.Range(0.8f, 1.8f);
                        float currentScale = instance.transform.localScale.x;
                        instance.transform.rotation *= Quaternion.Euler(Random.Range(-5, 5), 0, Random.Range(-5, 5));
                        instance.transform.rotation *= Quaternion.Euler(0, 0, 10);
                        instance.transform.localScale = new Vector3(scale * currentScale, scale * currentScale, scale * currentScale);
                        instance.transform.parent = transform;
                    }
                }
            }
        }
        yield return null;
    }
}
