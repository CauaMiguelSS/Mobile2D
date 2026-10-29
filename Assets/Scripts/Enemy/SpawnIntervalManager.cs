
using UnityEngine;

public class SpawnIntervalManager : MonoBehaviour
{
    public static SpawnIntervalManager Instance { get; private set; }

    [Header("Aceleração por inimigo morto")]
    [SerializeField] private float reducaoPorMorte = 0.1f;

    [Header("Intervalo mínimo")]
    [SerializeField] private float intervaloMinimo = 0.5f;

    private int inimigosMortos;

    public int InimigosMortos => inimigosMortos;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public float ObterIntervalo(float intervaloBase)
    {
        return Mathf.Max(
            intervaloMinimo,
            intervaloBase - inimigosMortos * reducaoPorMorte
        );
    }

    public void RegistrarMorte()
    {
        inimigosMortos++;
    }

    public void Reiniciar()
    {
        inimigosMortos = 0;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
