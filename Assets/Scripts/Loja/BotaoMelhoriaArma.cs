
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotaoMelhoriaArma : MonoBehaviour
{
    [Header("Identificação")]
    [SerializeField] private string idArma;

    [Header("Referências")]
    [SerializeField] private LojaManager loja;
    [SerializeField] private Button botao;
    [SerializeField] private TMP_Text textoNivel;
    [SerializeField] private TMP_Text textoPreco;

    private void Start()
    {
        if (botao == null)
            botao = GetComponent<Button>();

        botao.onClick.AddListener(Comprar);

        Atualizar();
    }

    private void Comprar()
    {
        loja.Comprar(idArma);
    }

    public void Atualizar()
    {
        int nivel = MelhoriasArmas.Nivel(idArma);

        if (nivel >= 10)
        {
            textoNivel.text = "Nível 10/10";
            textoPreco.text = "MAX";
            botao.interactable = false;
            return;
        }

        int preco = MelhoriasArmas.ProximoPreco(idArma);

        textoNivel.text = "Nível " + nivel + "/10";
        textoPreco.text = preco + " moedas";

        botao.interactable = EconomiaJogo.Saldo >= preco;
    }
}
