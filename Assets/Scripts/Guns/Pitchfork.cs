using UnityEngine;

public class Pitchfork : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 6f;

    [Header("Desvio do Rebote")]
    [SerializeField] private float bounceAngle = 67f;

    [Header("Dano")]
    [SerializeField] private float damagePerSecond = 20f;

    [Header("Layers")]
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D rb;
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 newDirection)
    {
        direction = newDirection.normalized;

        if (direction == Vector2.zero)
        {
            direction = Vector2.right;
        }

        rb.linearVelocity = direction * speed;

        RotateToDirection();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & wallLayer) != 0)
        {
            Rebater(other);
        }
    }

    private void Rebater(Collider2D parede)
    {
        Vector2 pontoMaisProximo = parede.ClosestPoint(transform.position);

        Vector2 normal =
            ((Vector2)transform.position - pontoMaisProximo).normalized;

        direction = Vector2.Reflect(direction, normal);

        float sinal = Random.value > 0.5f ? 1f : -1f;

        direction =
            Quaternion.Euler(0f, 0f, bounceAngle * sinal) * direction;

        direction.Normalize();

        rb.linearVelocity = direction * speed;

        RotateToDirection();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (other.gameObject.layer != enemyLayer)
            return;

        IDamageable enemy = other.GetComponent<IDamageable>();

        if (enemy != null)
        {
            enemy.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    private void RotateToDirection()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
