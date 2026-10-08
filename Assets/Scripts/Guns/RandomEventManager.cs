using UnityEngine;
using UnityEngine.UI;

public class RandomEventManager : MonoBehaviour
{
    public static RandomEventManager Instance { get; private set; }

    [Header("Barra")]
    [SerializeField] private Slider barra;

    [Header("Dano necessário")]
    [SerializeField] private float danoNecessario = 1000f;
    [SerializeField] private float aumentoPorEvento = 1.25f;

    [Header("Painel")]
    [SerializeField] private GameObject painel;

    [Header("Cartas")]
    [SerializeField] private BuffCard[] cartas;

    private float danoAtual;
    private bool eventoAberto;

    private void Awake()
    {
        Instance = this;

        if (painel != null)
            painel.SetActive(false);

        if (barra != null)
            barra.value = 0f;
    }

    public void RegistrarDano(float dano)
    {
        if (eventoAberto)
            return;

        danoAtual += dano;

        if (barra != null)
            barra.value = danoAtual / danoNecessario;

        if (danoAtual >= danoNecessario)
            AtivarEvento();
    }

    private void AtivarEvento()
    {
        eventoAberto = true;

        danoAtual = 0f;

        danoNecessario *= aumentoPorEvento;

        if (barra != null)
            barra.value = 0f;

        GerarCartas();

        Time.timeScale = 0f;

        painel.SetActive(true);
    }

    private void GerarCartas()
    {
        WeaponBuff[] armas =
            FindObjectsByType<WeaponBuff>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        int cartaAtual = 0;

        foreach (WeaponBuff arma in armas)
        {
            if (!arma.gameObject.activeSelf)
                continue;

            while (cartaAtual < cartas.Length)
            {
                TipoBuff tipo = SortearBuff(arma);

                int porcentagem = Random.Range(20, 101);

                cartas[cartaAtual].Configurar(
                    arma,
                    tipo,
                    porcentagem
                );

                cartaAtual++;
                break;
            }

            if (cartaAtual >= cartas.Length)
                break;
        }

        while (cartaAtual < cartas.Length)
        {
            WeaponBuff arma = EncontrarArmaDesbloqueada(armas);

            TipoBuff tipo = SortearBuff(arma);

            int porcentagem = Random.Range(20, 101);

            cartas[cartaAtual].Configurar(
                arma,
                tipo,
                porcentagem
            );

            cartaAtual++;
        }
    }

    private WeaponBuff EncontrarArmaDesbloqueada(WeaponBuff[] armas)
    {
        foreach (WeaponBuff arma in armas)
        {
            if (arma.gameObject.activeSelf)
                return arma;
        }

        return null;
    }

    private TipoBuff SortearBuff(WeaponBuff arma)
    {
        int quantidade = arma.PossuiIntervalo ? 4 : 3;

        int sorteio = Random.Range(0, quantidade);

        return (TipoBuff)sorteio;
    }

    public void EscolherBuff(
        WeaponBuff arma,
        TipoBuff tipo,
        int porcentagem)
    {
        arma.AplicarBuff(
            tipo,
            porcentagem / 100f
        );

        painel.SetActive(false);

        eventoAberto = false;

        Time.timeScale = 1f;
    }
}