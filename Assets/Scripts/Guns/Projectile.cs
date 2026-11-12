using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float velocidade = 10f;
    [SerializeField] private float dano = 10f;
    [Header("Tempo de vida")]
    [SerializeField] private float tempoDeVida = 3f;
    public AudioClip somDisparo;

    private bool ehClone;

    private Vector2 direcao;

    public void ConfigurarClone()
    {
        ehClone = true;
        Destroy(gameObject, tempoDeVida);
    }

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

        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (other.gameObject.layer != enemyLayer)
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
            damageable.TakeDamage(dano);

        Destroy(gameObject);
    }
    public void AplicarMelhoriaLoja(string idArma)
    {
        dano *= MelhoriasArmas.Multiplicador(idArma);
    }
}
