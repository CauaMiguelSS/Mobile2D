using UnityEngine;
using UnityEngine.UI;

public class BasicEnemy : MonoBehaviour, IDamageable
{
    [Header("Configurações")]
    [SerializeField] private float velocidade = 2f;

    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 30f;
    [SerializeField] private Slider barraVida;

    [Header("Referência")]
    [SerializeField] private Transform player;

    private float vidaAtual;

    private void Start()
    {
        vidaAtual = vidaMaxima;

        barraVida.value = 1f;
    }

    private void Update()
    {
        if (player == null)
            return;

        Vector2 direcao = (player.position - transform.position).normalized;

        transform.position += (Vector3)direcao * velocidade * Time.deltaTime;

        VirarParaPlayer();
    }

    public void TakeDamage(float damage)
    {
        vidaAtual -= damage;

        barraVida.value = vidaAtual / vidaMaxima;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    private void Morrer()
    {
        Destroy(gameObject);
    }

    private void VirarParaPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }
}