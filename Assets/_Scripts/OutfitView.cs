using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutfitView : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1000;
    [SerializeField] private int[] prices;
    [SerializeField] private string[] names;
    [SerializeField] private TMPro.TextMeshProUGUI priceTXT, nameTXT;
    [SerializeField] private GameObject selectButton, buyButton;
    [SerializeField] private int space = 4;
    
    private OutfitManager outfitManager;
    private GameObject[] items;
    private int index = 0;
    private float zoom = 3;

    void Start()
    {
        outfitManager = GameObject.Find("OutfitManager").GetComponent<OutfitManager>();
        items = new GameObject[transform.childCount];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = transform.GetChild(i).gameObject;
            items[i].transform.position = new Vector3(space * i, 0, 0);
        }

       /* for (int i = 0; i < items.Length; i++)
        {
            PlayerPrefs.SetInt("item" + items[i].name.ToString(), 0);
        }*/

        if (PlayerPrefs.GetInt("item" + items[0].name.ToString()) == 0)
            PlayerPrefs.SetInt("item" + items[0].name.ToString(), 1);
        UpdateView();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && index > 0)
        {
            index--;
            UpdateView();
        }   
        else if (Input.GetKeyDown(KeyCode.D) && index < items.Length - 1)
        {
            index++;
            UpdateView();
        }
        RotateItem();
    }

    private void UpdateView()
    {
        for (int i = 0; i < items.Length; i++)
        {
            int newX = i * space - index * space;
            if (newX == 0)
                items[i].transform.localScale = new Vector3(zoom, zoom, zoom);
            else
            {
                items[i].transform.rotation = Quaternion.identity;
                items[i].transform.localScale = new Vector3(1, 1, 1);
            }
            
            if (outfitManager.purchasedItems[index])
            {
                selectButton.SetActive(true);
                buyButton.SetActive(false);
            }
            else {
                selectButton.SetActive(false);
                buyButton.SetActive(true);
            }

            if (PlayerPrefs.GetInt("coins", 0) < prices[index])
                priceTXT.color = Color.red;
            else 
                priceTXT.color = Color.white;

            nameTXT.text = names[index];
            items[i].transform.position = new Vector3(newX, 0, 0);
            priceTXT.text = prices[index].ToString();
        }
    }

    private void RotateItem()
    {
        items[index].transform.Rotate(items[index].transform.rotation * new Vector3(0, Time.deltaTime * rotationSpeed, 0));
    }

    public void BuyItem()
    {
        int currentCoins = PlayerPrefs.GetInt("coins", 0);
        if (currentCoins >= prices[index])
        {
            PlayerPrefs.SetInt("coins", currentCoins - prices[index]);
            PlayerPrefs.SetInt("item" + items[index].name.ToString(), 1);
            outfitManager.purchasedItems[index] = true;
            UpdateView();
            outfitManager.UpdateCoins();
        }
        else
        {
            Debug.Log("Buy some coins!!");
        }
    }
    
    public void SelectItem()
    {
        PlayerPrefs.SetInt("activeItem", index);
        SceneManager.LoadScene(0);
    }

    public void LeftArrow()
    {
        if (index > 0)
        {
            index--;
            UpdateView();
        }
    }

    public void RightArrow()
    {
        if (index < items.Length - 1)
        {
            index++;
            UpdateView();
        }
    }
}
