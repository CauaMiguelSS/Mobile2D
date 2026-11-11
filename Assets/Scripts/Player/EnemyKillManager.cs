
using UnityEngine;
using TMPro;

public class EnemyKillManager : MonoBehaviour
{
    [Header("Baú")]
    [SerializeField] private GameObject bauPrefab;
    [SerializeField] private int mortesParaBaú = 10;
    [SerializeField] private int maximoDeBaus = 3;

    [Header("Pontuação")]
    [SerializeField] private TMP_Text textoPontos;
    [SerializeField] private int pontosPorInimigo = 100;

    [Header("Resultado da partida")]
    [SerializeField] private GameObject painelResultado;
    [SerializeField] private TMP_Text textoMoedasGanhos;
    [SerializeField] private TMP_Text textoSaldoTotal;

    private int inimigosMortos;
    private int bausCriados;
    private int pontos;
    private int moedas;

    private int saldoTotal;
    private bool partidaFinalizada;

    private const string CHAVE_MOEDAS = "MoedasJogador";

    private void Start()
    {
        saldoTotal = PlayerPrefs.GetInt(CHAVE_MOEDAS, 0);

        if (painelResultado != null)
            painelResultado.SetActive(false);

        AtualizarTexto();
    }

    public void InimigoMorreu(Vector3 posicao, int valorInimigo)
    {
        if (partidaFinalizada)
            return;

        inimigosMortos++;

        pontos += pontosPorInimigo;

        AtualizarTexto();

        if (inimigosMortos % mortesParaBaú == 0 &&
            bausCriados < maximoDeBaus)
        {
            CriarBau(posicao);
        }
    }

    public void FinalizarPartida()
    {
        if (partidaFinalizada)
            return;

        partidaFinalizada = true;

        moedas = pontos / 100;
        saldoTotal += moedas;

        PlayerPrefs.SetInt(CHAVE_MOEDAS, saldoTotal);
        PlayerPrefs.Save();

        if (painelResultado != null)
            painelResultado.SetActive(true);

        if (textoMoedasGanhos != null)
            textoMoedasGanhos.text = moedas.ToString();

        if (textoSaldoTotal != null)
            textoSaldoTotal.text = saldoTotal.ToString();

        Debug.Log("Moedas ganhas: " + moedas);
    }

    private void AtualizarTexto()
    {
        if (textoPontos != null)
            textoPontos.text = pontos.ToString();
    }

    private void CriarBau(Vector3 posicao)
    {
        if (bauPrefab == null)
            return;

        Instantiate(bauPrefab, posicao, Quaternion.identity);
        bausCriados++;
    }
}
