using UnityEngine;

public class AimingOrbit : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;

    [Header("Configurações")]
    [SerializeField] private float distancia = 1f;

    public Vector2 Direcao { get; private set; }

    public void SetDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        Direcao = direction.normalized;

        Vector3 novaPosicao =
            player.position + (Vector3)(Direcao * distancia);

        transform.position = novaPosicao;
    }
}
