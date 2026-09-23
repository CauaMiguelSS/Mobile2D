using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 100f;

    private float vidaAtual;

    private void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void TakeDamage(float damage)
    {
        vidaAtual -= damage;

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    private void Morrer()
    {
        Debug.Log("Player morreu!");
    }
}
