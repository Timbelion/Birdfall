using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class OutfitManager : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI coinsTXT;
    [SerializeField] private GameObject itemList;
    private GameObject[] items;
    private OutfitView outfitView;
    public bool[] purchasedItems;
    [SerializeField] private AudioMixer masterMixer;

    void Start()
    {
        UpdateCoins();
        outfitView = GameObject.Find("Items").GetComponent<OutfitView>();

        items = new GameObject[itemList.transform.childCount];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = itemList.transform.GetChild(i).gameObject;
        }

        purchasedItems = new bool[outfitView.transform.childCount];
        InitializePlayerItems();
        LoadVolumes();
    }

    public void InitializePlayerItems()
    {
        for (int i = 0; i < purchasedItems.Length; i++)
        {
            if (PlayerPrefs.GetInt("item" + items[i].name.ToString(), 0) == 1)
                purchasedItems[i] = true;
        }
        purchasedItems[0] = true;
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
