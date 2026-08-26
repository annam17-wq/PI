using UnityEngine;

public class BotaoSalvar : MonoBehaviour
{
    // Chame esse método no OnClick() do botão
    public void Salvar()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
        }
        else
        {
            Debug.LogWarning("GameManager não encontrado! O save não foi executado.");
        }
    }
}