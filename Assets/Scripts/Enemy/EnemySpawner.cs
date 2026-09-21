using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Inimigo")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Quantidade")]
    [SerializeField] private int quantidadeInimigos = 10;
    [SerializeField] private float intervaloSpawn = 1f;

    [Header("Escala de Vida")]
    [SerializeField] private float aumentoVidaPorSpawn = 0.10f;

    [Header("Pontos de Spawn")]
    [SerializeField] private Transform[] pontosSpawn;

    private int inimigosSpawnados;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, intervaloSpawn);
    }

    private void SpawnEnemy()
    {
        if (inimigosSpawnados >= quantidadeInimigos)
        {
            CancelInvoke(nameof(SpawnEnemy));
            return;
        }

        if (pontosSpawn == null || pontosSpawn.Length == 0)
        {
            CancelInvoke(nameof(SpawnEnemy));
            return;
        }

        int indice = Random.Range(0, pontosSpawn.Length);

        Transform ponto = pontosSpawn[indice];

        GameObject objetoInimigo = Instantiate(enemyPrefab, ponto.position, Quaternion.identity);

        BasicEnemy enemy = objetoInimigo.GetComponent<BasicEnemy>();

        if (enemy != null)
        {
            float multiplicador = Mathf.Pow(1f + aumentoVidaPorSpawn, inimigosSpawnados);

            float vidaEscalada = enemy.VidaMaximaBase * multiplicador;

            enemy.SetVidaMaxima(vidaEscalada);
        }

        inimigosSpawnados++;
    }
}
