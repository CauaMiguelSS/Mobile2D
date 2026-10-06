using UnityEngine;
using UnityEngine.UI;

public class RandomEventManager : MonoBehaviour
{
    [Header("Barra")]
    [SerializeField] private Slider barra;

    [Header("Evento")]
    [SerializeField] private float danoNecessario = 100f;
    [SerializeField] private float aumentoDanoNecessario = 50f;

    [Header("Painel")]
    [SerializeField] private GameObject painel;

    [SerializeField] private BuffCard[] cartas;

    private float danoAtual;

    public void ReceberDanoCausado(float dano)
    {
        danoAtual += dano;

        barra.value = danoAtual / danoNecessario;

        if (danoAtual >= danoNecessario)
        {
            AtivarEvento();
        }
    }

    private void AtivarEvento()
    {
        danoAtual = 0f;

        danoNecessario += aumentoDanoNecessario;

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

        System.Collections.Generic.List<WeaponBuff> desbloqueadas =
            new System.Collections.Generic.List<WeaponBuff>();

        foreach (WeaponBuff arma in armas)
        {
            if (arma.gameObject.activeSelf)
            {
                desbloqueadas.Add(arma);
            }
        }

        foreach (BuffCard carta in cartas)
        {
            WeaponBuff arma =
                desbloqueadas[
                    Random.Range(0, desbloqueadas.Count)
                ];

            string[] tipos =
            {
            "Dano",
            "Velocidade",
            "Tamanho",
            "Intervalo"
        };

            string tipo =
                tipos[Random.Range(0, tipos.Length)];

            float porcentagem =
                Random.Range(20, 101) / 100f;

            carta.Configurar(
                arma,
                tipo,
                porcentagem
            );
        }

    }
    public void FecharEvento()
    {
        painel.SetActive(false);
    }
}
