using UnityEngine;
using UnityEngine.Events;

// Colocar num objeto que fica SEMPRE ATIVO (ex.: o objeto "DialogoChave").
// Quando o objeto alvo (ex.: "Lugar dar chave") desaparece depois de ser coletado,
// executa os eventos configurados em "Ao Desaparecer" (ex.: abrir um diálogo).
public class ExecutarAoDesaparecer : MonoBehaviour
{
    [Header("Objeto que vai desaparecer ao ser coletado")]
    public GameObject alvo;

    [Header("Atraso antes de executar (segundos)")]
    public float atraso = 0.5f;

    [Header("O que fazer quando o alvo desaparecer")]
    public UnityEvent aoDesaparecer;

    bool viuAtivo;
    bool executou;

    void Update()
    {
        if (executou || alvo == null) return;

        if (alvo.activeInHierarchy)
        {
            viuAtivo = true;
            return;
        }

        // só dispara se o alvo já esteve ativo antes (evita disparar ao carregar com ele já coletado)
        if (viuAtivo)
        {
            executou = true;
            Invoke(nameof(Executar), atraso);
        }
    }

    void Executar()
    {
        aoDesaparecer.Invoke();
    }
}