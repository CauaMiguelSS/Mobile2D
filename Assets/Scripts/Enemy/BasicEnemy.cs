using UnityEngine;
using UnityEngine.UI;

public class BasicEnemy : MonoBehaviour, IDamageable
{
    [Header("Configurações")]
    [SerializeField] private float velocidade = 2f;

    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    [SerializeField] private Slider barraVida;

    [Header("Referência")]
    [SerializeField] private Transform player;

    [SerializeField] private bool inimigoPremiado;
    [SerializeField] private GameObject bauPrefab;

    private float vidaAtual;

    public float VidaMaximaBase => vidaMaxima;

    private void Awake()
    {
        vidaAtual = vidaMaxima;

        if (barraVida != null)
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

    public void SetVidaMaxima(float novaVida)
    {
        vidaMaxima = novaVida;
        vidaAtual = vidaMaxima;

        if (barraVida != null) 
        {
            barraVida.value = 1f;
        }
    }

    public void TakeDamage(float damage)
    {
        vidaAtual -= damage;

        if (barraVida != null)
        {
            barraVida.value = vidaAtual / vidaMaxima;
        }

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    private void Morrer()
    {
        if (inimigoPremiado && bauPrefab != null)
        {
            Instantiate(
                bauPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }

    private void VirarParaPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}