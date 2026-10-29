
using UnityEngine;

public static class EnemySpawnProgression
{
    private static int totalSpawns;

    public static float RegistrarSpawn(float aumentoVidaPorSpawn)
    {
        // O primeiro inimigo usa a vida base.
        float multiplicador = Mathf.Pow(
            1f + aumentoVidaPorSpawn,
            totalSpawns
        );

        totalSpawns++;

        return multiplicador;
    }

    public static void Reiniciar()
    {
        totalSpawns = 0;
    }
}

