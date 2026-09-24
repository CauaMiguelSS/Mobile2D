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
    [SerializeField] private int multiplicadorPontos = 100;

    private int inimigosMortos;
    private int bausCriados;
    private int pontos;

    private void Start()
    {
        AtualizarTexto();
    }

    public void InimigoMorreu(Vector3 posicao, int valorInimigo)
    {
        inimigosMortos++;

        pontos += valorInimigo * multiplicadorPontos;

        AtualizarTexto();

        if (inimigosMortos % mortesParaBaú == 0 && bausCriados < maximoDeBaus)
        {
            CriarBau(posicao);
        }
    }

    private void AtualizarTexto()
    {
        if (textoPontos != null)
        {
            textoPontos.text = pontos.ToString();
        }
    }

    private void CriarBau(Vector3 posicao)
    {
        if (bauPrefab == null)
            return;

        Instantiate(bauPrefab, posicao, Quaternion.identity);

        bausCriados++;
    }
}
