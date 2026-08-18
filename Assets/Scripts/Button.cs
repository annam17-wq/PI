using UnityEngine;

public class TrocaPainel : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject Esconde; // painel que vai sumir
    public GameObject Mostra; // painel que vai aparecer

    public void TrocarPainel()
    {
        Esconde.SetActive(false); // some
        Mostra.SetActive(true);  // exibe
    }
}