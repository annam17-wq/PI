using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SaveData currentData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Tenta carregar o jogo salvo
        currentData = SaveSystem.Load();

        // Se não existir save, cria dados novos
        if (currentData == null)
        {
            Debug.Log("Nenhum save encontrado. Criando novo jogo.");
            currentData = new SaveData();
        }

        // Garante que o inventário exista
        if (currentData.inventory == null)
        {
            currentData.inventory = new List<InventoryItem>();
        }

        // Garante que existam slots vazios
        if (currentData.inventory.Count == 0)
        {
            for (int i = 0; i < 20; i++)
            {
                currentData.inventory.Add(new InventoryItem
                {
                    itemId = ""
                });
            }
        }

        Debug.Log("GameManager inicializado. Slots: " +
                  currentData.inventory.Count);
    }

    public void SaveGame()
    {
        if (currentData == null)
        {
            Debug.LogError("Não é possível salvar: currentData está null!");
            return;
        }

        currentData.playerPosition = new Vector3Data(transform.position);
        SaveSystem.Save(currentData);

        Debug.Log("Jogo salvo manualmente!");
    }

    public bool AddItem(string id)
    {
        if (currentData == null || currentData.inventory == null)
        {
            Debug.LogError("Dados do jogo ou inventário não inicializados!");
            return false;
        }

        var inventario = currentData.inventory;

        var slotVazio = inventario.Find(
            i => i != null && string.IsNullOrEmpty(i.itemId)
        );

        if (slotVazio == null)
        {
            Debug.Log("Inventário cheio!");
            return false;
        }

        slotVazio.itemId = id;

        InventoryUI.Instance?.AtualizarUI();

        return true;
    }
}