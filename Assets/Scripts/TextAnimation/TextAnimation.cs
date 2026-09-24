using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    [Header("Objeto que será animado")]
    [SerializeField] private Transform objeto;

    [Header("Escala")]
    [SerializeField] private float escalaMinima = 0.95f;
    [SerializeField] private float escalaMaxima = 1.05f;
    [SerializeField] private float velocidadeEscala = 3f;

    [Header("Movimento")]
    [SerializeField] private float distanciaMovimento = 5f;
    [SerializeField] private float velocidadeMovimento = 2f;

    [Header("Rotação")]
    [SerializeField] private float anguloRotacao = 3f;
    [SerializeField] private float velocidadeRotacao = 2f;

    private Vector3 escalaOriginal;
    private Vector3 posicaoOriginal;

    private void Start()
    {
        if (objeto == null)
        {
            return;
        }

        escalaOriginal = objeto.localScale;
        posicaoOriginal = objeto.localPosition;
    }

    private void Update()
    {
        if (objeto == null)
            return;

        float tempo = Time.unscaledTime;

        AnimarEscala(tempo);
        AnimarMovimento(tempo);
        AnimarRotacao(tempo);
    }

    private void AnimarEscala(float tempo)
    {
        float valor = Mathf.Lerp(
            escalaMinima,
            escalaMaxima,
            (Mathf.Sin(tempo * velocidadeEscala) + 1f) / 2f
        );

        objeto.localScale = escalaOriginal * valor;
    }

    private void AnimarMovimento(float tempo)
    {
        float movimento =
            Mathf.Sin(tempo * velocidadeMovimento) * distanciaMovimento;

        objeto.localPosition =
            posicaoOriginal + Vector3.up * movimento;
    }

    private void AnimarRotacao(float tempo)
    {
        float rotacao =
            Mathf.Sin(tempo * velocidadeRotacao) * anguloRotacao;

        objeto.localRotation =
            Quaternion.Euler(0f, 0f, rotacao);
    }
}
