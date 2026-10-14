
using UnityEngine;

public class Shovel : MonoBehaviour
{
    [Header("Órbita")]
    [SerializeField] private Transform player;
    [SerializeField] private float distancia = 2f;
    [SerializeField] private float velocidadeOrbita = 180f;

    [Header("Dano")]
    [SerializeField] private float danoPorSegundo = 20f;

    private WeaponBuff buff;
    private Vector3 escalaOriginal;
    private float angulo;

    private void Start()
    {
        buff = GetComponent<WeaponBuff>();

        if (buff == null)
        {
            Debug.LogError("Shovel: WeaponBuff não encontrado!", gameObject);
            enabled = false;
            return;
        }

        if (player == null)
        {
            Debug.LogError("Shovel: Player não foi configurado!", gameObject);
            enabled = false;
            return;
        }

        escalaOriginal = transform.localScale;
    }

    private void Update()
    {
        angulo += velocidadeOrbita * buff.Velocidade * Time.deltaTime;

        float x = Mathf.Cos(angulo * Mathf.Deg2Rad) * distancia;
        float y = Mathf.Sin(angulo * Mathf.Deg2Rad) * distancia;

        transform.position = player.position + new Vector3(x, y, 0f);

        transform.Rotate(
            0f, 0f,
            velocidadeOrbita * buff.Velocidade * Time.deltaTime
        );

        transform.localScale = escalaOriginal * buff.Tamanho;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;

        IDamageable inimigo = other.GetComponent<IDamageable>();

        if (inimigo != null)
        {
            inimigo.TakeDamage(
                danoPorSegundo * buff.Dano * Time.deltaTime
            );
        }
    }
}
