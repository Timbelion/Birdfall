using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private TMPro.TextMeshProUGUI coinCounterTXT;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject deathPS, coinPS;
    [SerializeField] private float distance = 5;
    [SerializeField] private AudioSource coinPickup, hit;
    private AudioSource backgroundMusic;
    [SerializeField] private GameObject whoosh;
    private GameObject[] meshes;
    public float sideSpeed = 100, forwardSpeed = 100, xIncrement = 0.1f, rotationIncrement = 0.1f;
    public int coinCounter = 0;
    private float xMovement = 0;
    private Transform defaultTransform;
    private bool movingLeft = false, movingRight = false;
    private Vector3 yRotation;
    private float width, height;
    private bool leftWhoosh = false, rightWhoosh = false;

    void Start()
    {
        backgroundMusic = GameObject.Find("Music").GetComponent<AudioSource>();
        if (!backgroundMusic.isPlaying)
            backgroundMusic.Play();
        rigidbody = GetComponent<Rigidbody>();
        defaultTransform = transform;
        yRotation = defaultTransform.rotation.eulerAngles;

        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;

        LoadSkin();
    }

    private void LoadSkin()
    {
        int activeSkin = PlayerPrefs.GetInt("activeItem", 0);
        meshes = new GameObject[transform.Find("Meshes").transform.childCount];

        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i] = transform.Find("Meshes").transform.GetChild(i).gameObject;

            if (i != activeSkin)
                meshes[i].SetActive(false);
            else
                meshes[i].SetActive(true);
        }
    }

    void Update()
    {
        //                              MOBILE VERSION
        /*if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.position.x < width / 2)
            {
                movingLeft = true;
                movingRight = false;
            }
            else
            {
                movingRight = true;
                movingLeft = false;
            }
        }
        else
        {
            movingLeft = false;
            movingRight = false;
        }*/


        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            movingLeft = true;
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            movingLeft = false;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            movingRight = true;
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            movingRight = false;
        

        if (yRotation.x < -10 && !leftWhoosh)
        {
            leftWhoosh = true;
            var temp = Instantiate(whoosh, Vector3.zero, Quaternion.identity);
            Destroy(temp, 1);
        }
        else if (yRotation.x > 10 && !rightWhoosh)
        {
            rightWhoosh = true;
            var temp = Instantiate(whoosh, Vector3.zero, Quaternion.identity);
            Destroy(temp, 1);
        }
        
        if (yRotation.x > -10 && yRotation.x < 10)
        {
            rightWhoosh = false;
            leftWhoosh = false;
        }

        if (movingRight)
        {
            xMovement -= xIncrement * Time.deltaTime;
            yRotation.x -= rotationIncrement * Time.deltaTime;
        }
        else if (movingLeft)
        {
            xMovement += xIncrement * Time.deltaTime;
            yRotation.x += rotationIncrement * Time.deltaTime;
        }

        if (!movingLeft && !movingRight)
        {
            if (xMovement > 0.1f)
                xMovement -= xIncrement * Time.deltaTime;
            else if (xMovement < -0.1f)
                xMovement += xIncrement * Time.deltaTime;
            else
                xMovement = 0;

            if (yRotation.x > 2f)
                yRotation.x -= rotationIncrement * Time.deltaTime;
            else if (yRotation.x < -2f)
                yRotation.x += rotationIncrement * Time.deltaTime;
            else
                yRotation.x = 0;
        }


        xMovement = Mathf.Clamp(xMovement, -1, 1);
        yRotation.x = Mathf.Clamp(yRotation.x, -40, 40);
    }

    void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(yRotation));
        rigidbody.MovePosition(rigidbody.transform.position + new Vector3(defaultTransform.right.x, defaultTransform.right.y, 0) * forwardSpeed + new Vector3(0, 0, xMovement) * sideSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Tree" || collision.collider.tag == "SideTree")
        {
            collision.collider.tag = "Untagged";
            GameOver();
        }
    }

    private void GameOver()
    {
        hit.Play();
        backgroundMusic.Stop();
        if (!gameManager.adAccepted)
            StartCoroutine(gameManager.ContinueWithAd());
        else
        {
            gameManager.GameOver();
        }

        enabled = false;
        Instantiate(deathPS, transform.position, Quaternion.identity);
        transform.Find("Meshes").gameObject.SetActive(false);
    }

    public void ContinuePlaying()
    {
        backgroundMusic.Play();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        xMovement = 0;
        yRotation = Vector3.zero;
        movingLeft = false;
        movingRight = false;
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        for (int i = 0; i < trees.Length; i++)
        {
            if (Vector3.Distance(transform.position, trees[i].transform.position) < distance)
            {
                Destroy(trees[i]);
            }
        }
        transform.Find("Meshes").gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Coin")
        {
            other.tag = "Untagged";
            coinPickup.Play();
            coinCounter++;
            coinCounterTXT.text = coinCounter.ToString();
            var ps = Instantiate(coinPS, other.transform.position, Quaternion.identity);
            Destroy(other.transform.parent.gameObject);
            Destroy(ps, 2);
        }
    }
}
