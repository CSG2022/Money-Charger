using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
    public static CoinPool Instance;
    public List<GameObject> coinList;
    [SerializeField] int coinsInPool = 10;
    [SerializeField] GameObject coinPrefab;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < coinsInPool; i++)
        {
            var coin = Instantiate(coinPrefab);
            coin.SetActive(false);
            
            coinList.Add(coin);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < coinList.Count;i++)
        {
            if (!coinList[i].activeInHierarchy)
            {
                return coinList[i];
            }
        }
        return null;
    }
}
