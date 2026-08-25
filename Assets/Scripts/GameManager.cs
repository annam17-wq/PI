using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SaveData currentData;

    void Awake()
    {
        // garante que só existe um GameManager e que ele sobrevive entre cenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentData = SaveSystem.Load();
    }

    public void SaveGame()
    {
        currentData.playerPosition = new Vector3Data(transform.position);
        SaveSystem.Save(currentData);
    }

    public void AddItem(string id, int qty)
    {
        var existing = currentData.inventory.Find(i => i.itemId == id);
        if (existing != null)
            existing.quantity += qty;
        else
            currentData.inventory.Add(new InventoryItem { itemId = id, quantity = qty });
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }
}