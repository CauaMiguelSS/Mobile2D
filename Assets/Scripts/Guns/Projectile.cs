using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float velocidade = 10f;
    [SerializeField] private float tempoDeVida = 3f;
    [SerializeField] private float dano = 10f;

    private Vector2 direcao;

    public void SetDirection(Vector2 novaDirecao)
    {
        direcao = novaDirecao.normalized;

        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo);
    }

    private void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    private void Update()
    {
        transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (other.gameObject.layer != enemyLayer)
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(dano);
        }

        Destroy(gameObject);
    }
}
