
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Inimigo")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Quantidade")]
    [SerializeField] private int quantidadeInimigos = 10;
    [SerializeField] private float intervaloSpawn = 1f;

    [Header("Escala de Vida")]
    [Tooltip("0.02 = aumento de 2% por surgimento")]
    [SerializeField] private float aumentoVidaPorSpawn = 0.02f;

    [Header("Pontos de Spawn")]
    [SerializeField] private Transform[] pontosSpawn;

    private int inimigosSpawnados;
    private float proximoSpawn;

    private void Start()
    {
        proximoSpawn = IntervaloAtual();
    }

    private void Update()
    {
        if (inimigosSpawnados >= quantidadeInimigos)
            return;

        proximoSpawn -= Time.deltaTime;

        if (proximoSpawn <= 0f)
        {
            SpawnEnemy();
            proximoSpawn = IntervaloAtual();
        }
    }

    private float IntervaloAtual()
    {
        if (SpawnIntervalManager.Instance != null)
        {
            return SpawnIntervalManager.Instance.ObterIntervalo(
                intervaloSpawn
            );
        }

        return intervaloSpawn;
    }

    private void SpawnEnemy()
    {
        if (inimigosSpawnados >= quantidadeInimigos)
        {
            CancelInvoke(nameof(SpawnEnemy));
            return;
        }

        Transform ponto = pontosSpawn[
            Random.Range(0, pontosSpawn.Length)
        ];

        if (ponto == null)
            return;

        GameObject objetoInimigo = Instantiate(
            enemyPrefab,
            ponto.position,
            Quaternion.identity
        );

        float multiplicador = EnemySpawnProgression.RegistrarSpawn(aumentoVidaPorSpawn);

        bool vidaConfigurada = false;

        BasicEnemy basic = objetoInimigo.GetComponent<BasicEnemy>();
        FastEnemy fast = objetoInimigo.GetComponent<FastEnemy>();
        BossEnemy boss = objetoInimigo.GetComponent<BossEnemy>();

        if (basic != null)
        {
            basic.SetVidaMaxima(basic.VidaMaximaBase * multiplicador);
            vidaConfigurada = true;
        }
        else if (fast != null)
        {
            fast.SetVidaMaxima(fast.VidaMaximaBase * multiplicador);
            vidaConfigurada = true;
        }
        else if (boss != null)
        {
            boss.SetVidaMaxima(boss.VidaMaximaBase * multiplicador);
            vidaConfigurada = true;
        }

        inimigosSpawnados++;
    }
}
