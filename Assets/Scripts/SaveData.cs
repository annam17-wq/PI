using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string itemId;
}

[Serializable]
public class SaveData
{
    public int level;
    public float playTime;
    public Vector3Data playerPosition;

    // 5 posições fixas — itemId vazio ("") = slot vazio
    public List<InventoryItem> inventory = new List<InventoryItem>
    {
        new InventoryItem { itemId = "" },
        new InventoryItem { itemId = "" },
        new InventoryItem { itemId = "" },
        new InventoryItem { itemId = "" },
        new InventoryItem { itemId = "" },
    };

    public List<string> completedQuests = new List<string>();

    public float brilho = 1f;
    public int resolucaoIndex = -1;
    public bool telaCheia = true;
}

[Serializable]
public class Vector3Data
{
    public float x, y, z;

    public Vector3Data(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    public Vector3 ToVector3() => new Vector3(x, y, z);
}

