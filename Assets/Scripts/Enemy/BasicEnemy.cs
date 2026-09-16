using UnityEngine;

public class BasicEnemy : MonoBehaviour , IDamageable
{
    [Header("Configurações")]
    public float velocidade = 2f;

    [Header("Referência")]
    public Transform player;

    public void TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        if (player == null)
            return;

        Vector2 direcao = (player.position - transform.position).normalized;

        transform.position += (Vector3)direcao * velocidade * Time.deltaTime;

        VirarParaPlayer();
    }

    private void VirarParaPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }
}