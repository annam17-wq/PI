using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SaveData currentData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentData = SaveSystem.Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        currentData.playerPosition = new Vector3Data(transform.position);
        SaveSystem.Save(currentData);
        Debug.Log("Jogo salvo manualmente!");
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
        InventoryUI.Instance?.AtualizarUI();
        return true;
    }

    // OnApplicationQuit removido — o save agora só acontece
    // quando o jogador clica no botão de salvar
}