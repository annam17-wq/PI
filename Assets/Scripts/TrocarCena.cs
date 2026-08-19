using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocarCena : MonoBehaviour
{
    // Chame esse método no OnClick() do botão, digitando o NOME exato da cena
    // (a mesma cena precisa estar adicionada em File > Build Settings > Scenes In Build)
    public void CarregarCenaPorNome(string nomeCena)
    {
        SceneManager.LoadScene(nomeCena);
    }

    // Alternativa: carregar pelo índice da cena no Build Settings (0, 1, 2...)
    public void CarregarCenaPorIndice(int indice)
    {
        SceneManager.LoadScene(indice);
    }

    // Recarrega a cena atual (útil para botão de "Reiniciar")
    public void RecarregarCenaAtual()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Carrega a próxima cena na ordem do Build Settings
    public void CarregarProximaCena()
    {
        int proximoIndice = SceneManager.GetActiveScene().buildIndex + 1;

        if (proximoIndice < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(proximoIndice);
        else
            Debug.LogWarning("Não há próxima cena no Build Settings.");
    }
}