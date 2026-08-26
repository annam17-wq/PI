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

    public bool AddItem(string id)
    {
        var inventario = currentData.inventory;

        var slotVazio = inventario.Find(i => i.itemId == "");
        if (slotVazio == null)
        {
            Debug.Log("Inventário cheio!");
            return false;
        }

        slotVazio.itemId = id;
        Debug.Log($"Item '{id}' adicionado aos dados. InventoryUI.Instance é null? {InventoryUI.Instance == null}");

        InventoryUI.Instance?.AtualizarUI();
        return true;
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }
}