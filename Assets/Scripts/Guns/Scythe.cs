using UnityEngine;

public class Scythe : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private float distanciaMaxima = 3f;

    [Header("Dano")]
    [SerializeField] private float dano = 10f;
    [SerializeField] private float tempoVida = 3f;
    [SerializeField] private float velocidadeRotacao = 500f;

    private Vector2 direcao;
    private Vector3 posicaoInicial;
    private bool chegou;

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
        transform.Rotate(0f, 0f, velocidadeRotacao * Time.deltaTime);

        if (!chegou)
        {
            transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);

            float distanciaPercorrida = Vector3.Distance(posicaoInicial, transform.position);

            if (distanciaPercorrida >= distanciaMaxima)
            {
                chegou = true;
            }
        }
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