using UnityEngine;

public class Shovel : MonoBehaviour
{
    [Header("Órbita")]
    [SerializeField] private Transform player;
    [SerializeField] private float distancia = 2f;
    [SerializeField] private float velocidadeOrbita = 180f;

    [Header("Dano")]
    [SerializeField] private float danoPorSegundo = 20f;

    private float angulo;

    private void Update()
    {
        angulo += velocidadeOrbita * Time.deltaTime;

        float x = Mathf.Cos(angulo * Mathf.Deg2Rad) * distancia;
        float y = Mathf.Sin(angulo * Mathf.Deg2Rad) * distancia;

        transform.position = player.position + new Vector3(x, y, 0f);

        transform.Rotate(0f, 0f, velocidadeOrbita * Time.deltaTime);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (other.gameObject.layer != enemyLayer)
            return;

        IDamageable inimigo = other.GetComponent<IDamageable>();

        if (inimigo != null)
            inimigo.TakeDamage(danoPorSegundo * Time.deltaTime);
    }
}
