using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffCard : MonoBehaviour
{
    [SerializeField] private TMP_Text texto;
    [SerializeField] private Button botao;

    private WeaponBuff arma;
    private TipoBuff tipo;
    private int porcentagem;

    public void Configurar(
        WeaponBuff novaArma,
        TipoBuff novoTipo,
        int novaPorcentagem)
    {
        arma = novaArma;
        tipo = novoTipo;
        porcentagem = novaPorcentagem;

        texto.text =
            arma.NomeArma +
            "\n" +
            CriarDescricao();

        botao.onClick.RemoveAllListeners();

        botao.onClick.AddListener(Escolher);
    }

    private string CriarDescricao()
    {
        switch (tipo)
        {
            case TipoBuff.Dano:
                return "Dano +" + porcentagem + "%";

            case TipoBuff.Velocidade:
                return "Velocidade +" + porcentagem + "%";

            case TipoBuff.Tamanho:
                return "Tamanho +" + porcentagem + "%";

            case TipoBuff.Intervalo:
                return "Velocidade de tiro +" + porcentagem + "%";
        }

        return "";
    }

    private void Escolher()
    {
        RandomEventManager.Instance.EscolherBuff(
            arma,
            tipo,
            porcentagem
        );
    }
}