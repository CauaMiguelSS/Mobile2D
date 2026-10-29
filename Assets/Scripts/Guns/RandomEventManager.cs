
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomEventManager : MonoBehaviour
{
    public static RandomEventManager Instance { get; private set; }

    [Header("Barra")]
    [SerializeField] private Slider barra;

    [Header("Progressão")]
    [Min(1f)]
    [SerializeField] private float danoNecessario = 1000f;

    [Min(1f)]
    [SerializeField] private float aumentoPorEvento = 1.25f;

    [Header("Painel")]
    [SerializeField] private GameObject painel;

    [Header("Cartas")]
    [SerializeField] private BuffCard[] cartas;

    private float danoAtual;
    private bool eventoAberto;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;

        if (painel != null)
            painel.SetActive(false);

        if (barra != null)
            barra.value = 0f;

        LimparCartas();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void LimparCartas()
    {
        if (cartas == null)
            return;

        foreach (BuffCard carta in cartas)
        {
            if (carta != null)
                carta.Limpar();
        }
    }

    public void RegistrarDano(float dano)
    {
        if (eventoAberto || dano <= 0f)
            return;

        danoAtual += dano;

        if (barra != null)
            barra.value = Mathf.Clamp01(danoAtual / danoNecessario);

        if (danoAtual >= danoNecessario)
            AtivarEvento();
    }

    private void AtivarEvento()
    {
        if (painel == null || !GerarCartas())
            return;

        eventoAberto = true;
        danoAtual = 0f;
        danoNecessario *= aumentoPorEvento;

        if (barra != null)
            barra.value = 0f;

        painel.SetActive(true);
        Time.timeScale = 0f;
    }

    private bool GerarCartas()
    {
        LimparCartas();

        if (cartas == null || cartas.Length == 0)
            return false;

        WeaponBuff[] encontradas =
            FindObjectsByType<WeaponBuff>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );

        List<WeaponBuff> armasValidas = new List<WeaponBuff>();

        foreach (WeaponBuff arma in encontradas)
        {
            if (arma == null || !arma.gameObject.activeInHierarchy)
                continue;

            if (!armasValidas.Contains(arma))
                armasValidas.Add(arma);
        }

        if (armasValidas.Count == 0)
            return false;

        int configuradas = 0;

        foreach (BuffCard carta in cartas)
        {
            if (carta == null)
                continue;

            WeaponBuff arma = armasValidas[
                Random.Range(0, armasValidas.Count)
            ];

            int quantidadeBuffs = arma.PossuiIntervalo ? 4 : 3;
            TipoBuff tipo = (TipoBuff)Random.Range(0, quantidadeBuffs);
            int porcentagem = Random.Range(10, 40);

            carta.Configurar(arma, tipo, porcentagem);

            if (carta.EstaConfigurada)
                configuradas++;
        }

        return configuradas > 0;
    }

    public void EscolherBuff(
        WeaponBuff arma,
        TipoBuff tipo,
        int porcentagem)
    {
        if (!eventoAberto || arma == null)
            return;

        if (!arma.gameObject.activeInHierarchy)
            return;

        arma.AplicarBuff(tipo, porcentagem / 100f);

        eventoAberto = false;

        if (painel != null)
            painel.SetActive(false);

        Time.timeScale = 1f;
    }
}
