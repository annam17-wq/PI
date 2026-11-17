using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fala
{
    public string nome;
    [TextArea(2, 5)] public string texto;
    public Sprite spriteFechado; // boca fechada (também é o sprite quando o texto termina)
    public Sprite spriteAberto;  // boca aberta (alterna com o fechado enquanto fala)
}

[CreateAssetMenu(fileName = "NovoDialogo", menuName = "Jogo/Dialogo")]
public class DialogoData : ScriptableObject
{
    [Tooltip("Se marcado, o diálogo só toca uma vez por execução do jogo")]
    public bool soUmaVez = true;
    public List<Fala> falas = new List<Fala>();
}