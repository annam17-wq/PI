using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Coloque no objeto "ConteudoTerminal" (dentro do PainelZoom).
// Enquanto o terminal esta aberto, o jogador digita no teclado e aperta Enter.
// As senhas valem em qualquer ordem, sem diferenciar maiusculas, minusculas e acentos.
public class TerminalSenhas : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Texto (TextMeshPro) que fica em cima da tela do monitor")]
    public TMP_Text textoTerminal;

    [Tooltip("Opcional: campo onde o jogador digita (TMP Input Field). Vazio = o script le o teclado direto e mostra tudo no Texto Terminal")]
    public TMP_InputField campoEntrada;

    [Header("Senhas (escreva sem acento e em minusculas)")]
    public string[] senhas = { "lacio", "roma", "cantelmo" };

    [Header("Mensagens")]
    public string prompt = "> ";
    public string mensagemInicial = "Digite a senha e aperte Enter.";
    public string mensagemSenhaErrada = "Senha incorreta.";
    public string mensagemSenhaRepetida = "Senha ja inserida.";

    [Header("Ajustes")]
    [Tooltip("Maximo de linhas guardadas. A altura da caixa do Texto Terminal tambem limita: frases antigas somem ate caber")]
    public int maxLinhas = 8;
    [Tooltip("Maximo de letras que o jogador consegue digitar")]
    public int maxCaracteres = 24;
    public float intervaloCursor = 0.5f;

    [Header("Quando acertar todas as senhas")]
    public UnityEvent aoConcluir;

    private readonly List<string> linhas = new List<string>();
    private readonly HashSet<string> acertadas = new HashSet<string>();
    private string digitado = "";
    private bool concluido;
    private bool cursorVisivel = true;
    private float tempoCursor;
    private bool precisaRedesenhar = true;

    public bool Concluido
    {
        get { return concluido; }
    }

    void OnEnable()
    {
        if (linhas.Count == 0 && !string.IsNullOrEmpty(mensagemInicial))
            linhas.Add(mensagemInicial);

        if (campoEntrada != null)
        {
            campoEntrada.characterLimit = maxCaracteres;
            campoEntrada.text = "";
            campoEntrada.onSubmit.AddListener(AoEnviarCampo);
            if (!concluido) campoEntrada.ActivateInputField();
        }

        precisaRedesenhar = true;
        Redesenhar();
    }

    void OnDisable()
    {
        if (campoEntrada != null)
            campoEntrada.onSubmit.RemoveListener(AoEnviarCampo);
    }

    // Enter apertado dentro do campo de entrada
    private void AoEnviarCampo(string texto)
    {
        if (concluido) return;

        digitado = texto;
        Enviar();
        campoEntrada.text = "";
    }

    void Update()
    {
        if (textoTerminal == null) return;

        // Sem campo de entrada, nenhum botao pode ficar selecionado,
        // senao o Enter do jogador aperta esse botao (ex.: o de pause)
        if (campoEntrada == null && !concluido &&
            EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (!concluido && campoEntrada != null)
        {
            // Com campo de entrada: so garante que ele continue selecionado para digitar
            // (depois do Enter o campo perde o foco sozinho)
            if (!campoEntrada.isFocused) campoEntrada.ActivateInputField();
        }
        else if (!concluido)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b')
                {
                    if (digitado.Length > 0)
                    {
                        digitado = digitado.Substring(0, digitado.Length - 1);
                        precisaRedesenhar = true;
                    }
                }
                else if (c == '\n' || c == '\r')
                {
                    Enviar();
                }
                else if (!char.IsControl(c) && digitado.Length < maxCaracteres)
                {
                    digitado += c;
                    precisaRedesenhar = true;
                }
            }

            // Cursor piscando
            tempoCursor += Time.unscaledDeltaTime;
            if (tempoCursor >= intervaloCursor)
            {
                tempoCursor = 0f;
                cursorVisivel = !cursorVisivel;
                precisaRedesenhar = true;
            }
        }

        if (precisaRedesenhar) Redesenhar();
    }

    private void Enviar()
    {
        string tentativa = Normalizar(digitado);

        AdicionarLinha(prompt + digitado);
        digitado = "";

        if (tentativa.Length == 0)
        {
            precisaRedesenhar = true;
            return;
        }

        int indice = IndiceDaSenha(tentativa);

        if (indice < 0)
        {
            AdicionarLinha(mensagemSenhaErrada);
        }
        else if (acertadas.Contains(Normalizar(senhas[indice])))
        {
            AdicionarLinha(mensagemSenhaRepetida);
        }
        else
        {
            acertadas.Add(Normalizar(senhas[indice]));

            int total = senhas.Length;
            int n = acertadas.Count;

            if (n >= total)
            {
                AdicionarLinha("Senha " + n + "/" + total + ", tarefa concluida com sucesso.");
                concluido = true;
                if (campoEntrada != null) campoEntrada.interactable = false;
                Debug.Log("Terminal: todas as senhas foram inseridas!");
                if (aoConcluir != null) aoConcluir.Invoke();
            }
            else
            {
                AdicionarLinha("Senha " + n + "/" + total);
            }
        }

        precisaRedesenhar = true;
    }

    private int IndiceDaSenha(string tentativa)
    {
        for (int i = 0; i < senhas.Length; i++)
        {
            if (Normalizar(senhas[i]) == tentativa) return i;
        }
        return -1;
    }

    private void AdicionarLinha(string linha)
    {
        linhas.Add(linha);

        // Limite fixo de linhas guardadas (a altura da caixa limita de novo no Redesenhar)
        while (linhas.Count > Mathf.Max(1, maxLinhas - 1))
            linhas.RemoveAt(0);
    }

    private string MontarTexto()
    {
        var sb = new StringBuilder();

        foreach (string linha in linhas)
            sb.Append(linha).Append('\n');

        // Com campo de entrada, a linha de digitacao e o proprio campo
        if (!concluido && campoEntrada == null)
        {
            sb.Append(prompt).Append(digitado);
            if (cursorVisivel) sb.Append('_');
        }

        return sb.ToString();
    }

    private void Redesenhar()
    {
        precisaRedesenhar = false;
        if (textoTerminal == null) return;

        textoTerminal.text = MontarTexto();

        // Apaga a frase mais antiga ate o texto caber na altura da caixa
        float alturaMax = textoTerminal.rectTransform.rect.height;
        while (linhas.Count > 0 && textoTerminal.preferredHeight > alturaMax)
        {
            linhas.RemoveAt(0);
            textoTerminal.text = MontarTexto();
        }
    }

    // minusculas, sem espacos nas pontas e sem acentos (Lacio = lacio = Lácio = lácio)
    private static string Normalizar(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return "";

        string decomposto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (char c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}