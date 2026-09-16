using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int quantidadeInimigos = 10;
    [SerializeField] private float intervaloSpawn = 1f;

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

        Instantiate(enemyPrefab, ponto.position, Quaternion.identity);

        inimigosSpawnados++;
    }
}
