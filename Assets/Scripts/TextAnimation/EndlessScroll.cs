
using UnityEngine;

public class EndlessScroll : MonoBehaviour
{
    [Header("Cenarios")]
    [SerializeField] private Transform cenarioA;
    [SerializeField] private Transform cenarioB;

    [Header("Movimento")]
    [SerializeField] private float velocidade = 3f;

    [Header("Reset")]
    [SerializeField] private float distanciaParaResetar = 20f;

    private float distanciaA;
    private float distanciaB;

    private Vector3 posicaoInicialA;
    private Vector3 posicaoInicialB;

    private void Start()
    {
        posicaoInicialA = cenarioA.position;
        posicaoInicialB = cenarioB.position;
    }

    private void Update()
    {
        float movimento = velocidade * Time.deltaTime;

        cenarioA.position += Vector3.up * movimento;
        cenarioB.position += Vector3.up * movimento;

        distanciaA += movimento;
        distanciaB += movimento;

        if (distanciaA >= distanciaParaResetar)
        {
            cenarioA.position = posicaoInicialA;
            distanciaA = 0f;
        }

        if (distanciaB >= distanciaParaResetar)
        {
            cenarioB.position = posicaoInicialB;
            distanciaB = 0f;
        }
    }
}
