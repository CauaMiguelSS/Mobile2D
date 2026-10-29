
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float velocidade = 10f;
    [SerializeField] private float dano = 10f;

    private Vector2 direcao;

    public void SetDirection(Vector2 novaDirecao)
    {
        direcao = novaDirecao.normalized;

        if (direcao.sqrMagnitude <= 0f)
            return;

        float angulo = Mathf.Atan2(
            direcao.y,
            direcao.x
        ) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angulo);
    }

    public void AplicarBuff(WeaponBuff buff)
    {
        if (buff == null)
            return;

        dano *= buff.Dano;
        velocidade *= buff.Velocidade;
        transform.localScale *= buff.Tamanho;
    }

    private void Update()
    {
        transform.position +=
            (Vector3)(direcao * velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Limite"))
        {
            Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Enemy"))
            return;

        IDamageable alvo = other.GetComponent<IDamageable>();

        if (alvo != null)
            alvo.TakeDamage(dano);

        Destroy(gameObject);
    }
}
