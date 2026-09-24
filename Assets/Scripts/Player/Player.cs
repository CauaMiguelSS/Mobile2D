using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Morte")]
    [SerializeField] private Sprite spriteMorte;
    [SerializeField] private GameObject painelMorte;

    [Header("Referências")]
    [SerializeField] private AimingOrbit aimingOrbit;

    private bool morreu;

    private void Start()
    {
        painelMorte.SetActive(false);
    }

    private void Update()
    {
        VirarParaMira();
    }

    private void VirarParaMira()
    {
        if (aimingOrbit.Direcao.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (aimingOrbit.Direcao.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
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
