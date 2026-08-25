using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string itemId;
    public int quantity;
}

[Serializable]
public class SaveData
{
    public int level;
    public float playTime;
    public Vector3Data playerPosition;
    public List<InventoryItem> inventory = new List<InventoryItem>();
    public List<string> completedQuests = new List<string>();

    // ---------- CONFIGURAÇÕES ----------
    public float brilho = 1f;          // valor padrão: slider no máximo
    public int resolucaoIndex = -1;    // -1 = ainda não definido, usa a atual da tela
    public bool telaCheia = true;
}

[Serializable]
public class Vector3Data
{
    public float x, y, z;

    public Vector3Data(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    public Vector3 ToVector3() => new Vector3(x, y, z);
}