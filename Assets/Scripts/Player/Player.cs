using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Morte")]
    [SerializeField] private Sprite spriteMorte;
    [SerializeField] private GameObject painelMorte;

    [Header("Referências")]
    [SerializeField] private AimingOrbit aimingOrbit;

    private SpriteRenderer spriteRenderer;

    private bool morreu;

    private void Start()
    {
        painelMorte.SetActive(false);
    }

    private void Update()
    {
        if (aimingOrbit.Direcao.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (aimingOrbit.Direcao.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (collision.gameObject.layer != enemyLayer)
            return;

        Morrer();
    }

    private void Morrer()
    {
        if (morreu)
            return;

        morreu = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && spriteMorte != null)
        {
            spriteRenderer.sprite = spriteMorte;
        }

        painelMorte.SetActive(true);

        Time.timeScale = 0f;
    }
}
