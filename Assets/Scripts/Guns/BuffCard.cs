
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffCard : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TMP_Text texto;
    [SerializeField] private Button botao;

    private WeaponBuff arma;
    private TipoBuff tipo;
    private int porcentagem;
    private bool configurada;

    public bool EstaConfigurada =>
        configurada && arma != null;

    private void Awake()
    {
        if (botao == null)
            botao = GetComponent<Button>();

        if (botao != null)
        {
            botao.onClick.RemoveListener(Escolher);
            botao.onClick.AddListener(Escolher);
        }
    }

    public void Configurar(
        WeaponBuff novaArma,
        TipoBuff novoTipo,
        int novaPorcentagem)
    {
        if (novaArma == null || texto == null || botao == null)
        {
            Limpar();
            return;
        }

        arma = novaArma;
        tipo = novoTipo;
        porcentagem = novaPorcentagem;
        configurada = true;

        texto.text = $"{arma.NomeArma}\n{CriarDescricao()}";
        botao.interactable = true;
    }

    public void Limpar()
    {
        configurada = false;
        arma = null;
        porcentagem = 0;

        if (texto != null)
            texto.text = "";

        if (botao != null)
            botao.interactable = false;
    }

    private string CriarDescricao()
    {
        switch (tipo)
        {
            case TipoBuff.Dano:
                return $"Dano +{porcentagem}%";

            case TipoBuff.Velocidade:
                return $"Velocidade +{porcentagem}%";

            case TipoBuff.Tamanho:
                return $"Tamanho +{porcentagem}%";

            case TipoBuff.Intervalo:
                return $"Intervalo de tiro +{porcentagem}%";

            default:
                return "";
        }
    }

    private void Escolher()
    {
        if (!EstaConfigurada)
            return;

        if (RandomEventManager.Instance == null)
            return;

        RandomEventManager.Instance.EscolherBuff(
            arma,
            tipo,
            porcentagem
        );
    }

    private void OnDestroy()
    {
        if (botao != null)
            botao.onClick.RemoveListener(Escolher);
    }
}
