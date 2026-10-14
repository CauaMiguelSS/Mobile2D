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
        WeaponBuff[] todasAsArmas =
            FindObjectsByType<WeaponBuff>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        System.Collections.Generic.List<WeaponBuff> armasDisponiveis =
            new System.Collections.Generic.List<WeaponBuff>();

        foreach (WeaponBuff arma in todasAsArmas)
        {
            if (arma == null)
                continue;

            if (!arma.gameObject.activeInHierarchy)
                continue;

            armasDisponiveis.Add(arma);
        }

        if (armasDisponiveis.Count == 0)
        {
            Debug.LogError("RandomEventManager: nenhuma arma desbloqueada com WeaponBuff!");
            return;
        }

        for (int i = 0; i < cartas.Length; i++)
        {
            WeaponBuff arma = armasDisponiveis[
                Random.Range(0, armasDisponiveis.Count)
            ];

            TipoBuff tipo = SortearBuff(arma);
            int porcentagem = Random.Range(20, 101);

            cartas[i].Configurar(arma, tipo, porcentagem);

            Debug.Log(
                $"Carta {i + 1}: {arma.NomeArma} | " +
                $"Objeto: {arma.gameObject.name} | " +
                $"Buff: {tipo} +{porcentagem}%"
            );
        }
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
        if (arma == null)
        {
            Debug.LogError("A arma selecionada não possui WeaponBuff!");
            return;
        }

        arma.AplicarBuff(tipo, porcentagem / 100f);

        painel.SetActive(false);
        eventoAberto = false;
        Time.timeScale = 1f;
    }
}