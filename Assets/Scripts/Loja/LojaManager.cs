
using UnityEngine;
using TMPro;

public class LojaManager : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private TMP_Text textoMoedas;

    [SerializeField] private BotaoMelhoriaArma[] botoesArmas;

    private void Start()
    {
        AtualizarLoja();
    }

    public void Comprar(string idArma)
    {
        bool comprou = MelhoriasArmas.Melhorar(idArma);

        if (comprou)
        {
            Debug.Log("Melhoria comprada: " + idArma);
        }
        else
        {
            Debug.Log("Moedas insuficientes ou nível máximo!");
        }

        AtualizarLoja();
    }

    public void AtualizarLoja()
    {
        if (textoMoedas != null)
            textoMoedas.text = EconomiaJogo.Saldo.ToString();

        foreach (BotaoMelhoriaArma botao in botoesArmas)
        {
            if (botao != null)
                botao.Atualizar();
        }
    }
}
