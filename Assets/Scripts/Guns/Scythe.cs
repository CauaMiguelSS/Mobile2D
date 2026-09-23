using UnityEngine;

public class Scythe : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private float distanciaMaxima = 5f;
    [SerializeField] private float tempoVida = 3f;
    [SerializeField] private float velocidadeRotacao = 500f;

    [Header("Dano")]
    [SerializeField] private float dano = 10f;

    private Vector2 direcao;
    private Vector3 posicaoInicial;
    private bool parada;

    public void Lancar(Vector2 novaDirecao)
    {
        direcao = novaDirecao.normalized;
        posicaoInicial = transform.position;
    }

    private void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    private void Update()
    {
        if (!parada)
        {
            transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);

            float distanciaPercorrida = Vector3.Distance(posicaoInicial, transform.position);

            if (distanciaPercorrida >= distanciaMaxima)
            {
                parada = true;
            }
        }

        transform.Rotate(0f, 0f, velocidadeRotacao * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (other.gameObject.layer != enemyLayer)
            return;

        parada = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (other.gameObject.layer != enemyLayer)
            return;

        IDamageable inimigo = other.GetComponent<IDamageable>();

        if (inimigo != null)
        {
            inimigo.TakeDamage(dano * Time.deltaTime);
        }
    }
}