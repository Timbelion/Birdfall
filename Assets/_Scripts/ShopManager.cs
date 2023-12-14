using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI coinsTXT;
    [SerializeField] private AudioMixer masterMixer;

    void Start()
    {
        UpdateCoins();
        LoadVolumes();
    }
    
    public void WatchAd(int amount)
    {
        // DISPLAY AD
        Debug.Log("Ad watched");
        PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins", 0) + amount);
        UpdateCoins();
    }

    public void UpdateCoins()
    {
        coinsTXT.text = PlayerPrefs.GetInt("coins", 0).ToString();
    }

    public void BackToGame()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadVolumes()
    {
        if (PlayerPrefs.GetInt("Music", 1) == 1)
        {
            masterMixer.SetFloat("Music", 0);
        }
        else
        {
            masterMixer.SetFloat("Music", -80);
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 1)
        {
            masterMixer.SetFloat("SFX", 0);
        }
        else
        {
            masterMixer.SetFloat("SFX", -80);
        }
    }
}
