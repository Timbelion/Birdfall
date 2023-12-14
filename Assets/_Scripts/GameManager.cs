using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public float score = 0;
    [SerializeField] private TMPro.TextMeshProUGUI scoreTXT, gameBestTXT, gameOverBestTXT, newBestTXT, currentTXT, gameCoinsTXT, gameOverCoinsTXT, totalCoinsTXT, soundsTXT, musicTXT;
    [SerializeField] private TMPro.TextMeshProUGUI adTimer;
    [SerializeField] private GameObject player;
    [SerializeField] private Canvas gameOverCanvasWEB, gameCanvasWEB, startCanvasWEB, watchAdCanvasWEB, settingsCanvasWEB;
    [SerializeField] private Camera camera;
    [SerializeField] private float adChanceDuration = 3;
    [SerializeField] private Material terrainMaterial;
    [SerializeField] private AudioMixer masterMixer;
    private bool diffPass1 = false, diffPass2 = false, diffPass3 = false, diffPass4 = false, diffPass5 = false, diffPass6 = false;
    public bool platfrom = false;
    public int density = 20;
    private float diff = 0;
    private PlayerMovement playerMovement;
    public bool gameOver = false;
    public bool adAccepted = false;
    private bool canStart = false;
    private string[] terrainColors = {"FFFFFF", "2EA174", "2E99A1", "2E6AA1", "2E35A1", "842EA1", "A12E84", "A12E55", "A1302E", "A15F2E", "A1932E", "7EA12E", "4FA12E"};

    // false = WEBGL
    // true = Android

    void Start()
    {
        //PlayerPrefs.SetInt("coins", 0);

        Application.targetFrameRate = 60;
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.orientation = ScreenOrientation.Portrait;
            platfrom = true;
        }
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            platfrom = false;
        }

        LoadVolumes();

        Color color;
        ColorUtility.TryParseHtmlString("#" + terrainColors[Random.Range(0, terrainColors.Length - 1)], out color);
        
        terrainMaterial.color = color;

        gameCanvasWEB.gameObject.SetActive(false);
        gameOverCanvasWEB.gameObject.SetActive(false);
        startCanvasWEB.gameObject.SetActive(true);
        gameBestTXT.text = "<size=72>BEST </size>\n" + PlayerPrefs.GetInt("score").ToString();
        totalCoinsTXT.text = PlayerPrefs.GetInt("coins", 0).ToString();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.enabled = false;
    }

    void Update()
    {
        UpdateScore();
        UpdateDifficulty();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!gameOver)
            {
                startCanvasWEB.gameObject.SetActive(false);
                gameCanvasWEB.gameObject.SetActive(true);
                playerMovement.enabled = true;
            }
            else if (canStart)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    private void UpdateScore()
    {
        score = player.transform.position.x;
        scoreTXT.text = ((int)score).ToString();
    }

    public bool SaveScore()
    {
        if ((int)score > PlayerPrefs.GetInt("score"))
        {
            PlayerPrefs.SetInt("score", (int)score);
            return true;
        }
        return false;
    }

    public IEnumerator ContinueWithAd()
    {
        gameOver = true;

        gameCanvasWEB.gameObject.SetActive(false);
        watchAdCanvasWEB.gameObject.SetActive(true);   

        float start = Time.time;
        while (Time.time - start < adChanceDuration && !adAccepted)
        {
            adTimer.text = ((int)(adChanceDuration - (Time.time - start - 1))).ToString();
            yield return null;
        }

        if (!adAccepted)
            GameOver();
    }

    public void GameOver()
    {
        canStart = true;
        PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins", 0) + playerMovement.coinCounter);
        watchAdCanvasWEB.gameObject.SetActive(false);
        gameCanvasWEB.gameObject.SetActive(false);
        gameOverCanvasWEB.gameObject.SetActive(true);

        bool saveResult = SaveScore();

        gameOverCoinsTXT.text = gameCoinsTXT.text;

        if (saveResult) // NEW HIGHSCORE
        {
            gameOverCanvasWEB.transform.Find("Current").gameObject.SetActive(false);
            gameOverCanvasWEB.transform.Find("Best").gameObject.SetActive(false);
            newBestTXT.text = "<size=100><color=#FFC000>NEW BEST\n</size></color> " + ((int)score).ToString();
        }
        else
        {
            gameOverCanvasWEB.transform.Find("NewBest").gameObject.SetActive(false);
            currentTXT.text = ((int)score).ToString();
            gameOverBestTXT.text = "<size=50>BEST</size> " + PlayerPrefs.GetInt("score").ToString();
        }
    }

    public void UpdateDifficulty()
    {
        if (score > 100 && playerMovement.forwardSpeed < 0.3)
            playerMovement.forwardSpeed += (Time.deltaTime / 1000);

        if (score > 100 && camera.fieldOfView < 80)
            camera.fieldOfView += (Time.deltaTime / 10);

        if (score > 200 && !diffPass1)
        {
            diffPass1 = true;
            density = 25;
        }
        if (score > 500 && !diffPass2)
        {
            diffPass2 = true;
            density = 30;
        }
        if (score > 1000 && !diffPass3)
        {
            diffPass3 = true;
            density = 35;
        }
    }

    public void Restart()
    {
        StartCoroutine(RestartWithDelay());
    }

    private IEnumerator RestartWithDelay()
    {
        yield return new WaitForSeconds(.1f);
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        startCanvasWEB.gameObject.SetActive(false);
        gameCanvasWEB.gameObject.SetActive(true);
        playerMovement.enabled = true;
    }

    public void GoToShop()
    {
        StartCoroutine(GoToShopWithDelay());
    }

    public void GoToSettings()
    {
        startCanvasWEB.gameObject.SetActive(false);
        settingsCanvasWEB.gameObject.SetActive(true);
    }

    public void BackToGame()
    {
        startCanvasWEB.gameObject.SetActive(true);
        settingsCanvasWEB.gameObject.SetActive(false);
    }

    public void GoToOutfits()
    {
        StartCoroutine(GoToOutfitsWithDelay());
    }


    private IEnumerator GoToShopWithDelay()
    {
        yield return new WaitForSeconds(.1f);
        SceneManager.LoadScene(2);
    }

    private IEnumerator GoToOutfitsWithDelay()
    {
        yield return new WaitForSeconds(.1f);
        SceneManager.LoadScene(1);
    }

    public void ContinuePlaying()
    {
        playerMovement.enabled = true;
        gameCanvasWEB.gameObject.SetActive(true);
        watchAdCanvasWEB.gameObject.SetActive(false);
        playerMovement.ContinuePlaying();
        adAccepted = true;
        Debug.Log("Continue playing!");
    }

    public void LoadVolumes()
    {
        if (PlayerPrefs.GetInt("Music", 1) == 1)
        {
            masterMixer.SetFloat("Music", 0);
            musicTXT.text = "Music ON";
        }
        else
        {
            masterMixer.SetFloat("Music", -80);
            musicTXT.text = "Music OFF";
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 1)
        {
            masterMixer.SetFloat("SFX", 0);
            soundsTXT.text = "Sounds ON";
        }
        else
        {
            masterMixer.SetFloat("SFX", -80);
            soundsTXT.text = "Sounds OFF";
        }
    }

    public void ToggleSFX()
    {
        if (PlayerPrefs.GetInt("SFX", 1) == 1) // MUSIC IS CURRENTLY ON AND WE TURN IT OFF
        {
            PlayerPrefs.SetInt("SFX", 0);
            masterMixer.SetFloat("SFX", -80);
            soundsTXT.text = "Sounds OFF";
        }
        else if (PlayerPrefs.GetInt("SFX", 1) == 0) // MUSIC IS CURRENTLY OFF AND WE TURN IT ON
        {
            PlayerPrefs.SetInt("SFX", 1);
            masterMixer.SetFloat("SFX", 0);
            soundsTXT.text = "Sounds ON";
        }
    }

    public void ToggleMusic()
    {
        if (PlayerPrefs.GetInt("Music", 1) == 1) // MUSIC IS CURRENTLY ON AND WE TURN IT OFF
        {
            PlayerPrefs.SetInt("Music", 0);
            masterMixer.SetFloat("Music", -80);
            musicTXT.text = "Music OFF";
        }
        else if (PlayerPrefs.GetInt("Music", 1) == 0) // MUSIC IS CURRENTLY OFF AND WE TURN IT ON
        {
            PlayerPrefs.SetInt("Music", 1);
            masterMixer.SetFloat("Music", 0);
            musicTXT.text = "Music ON";
        }
    }

}
