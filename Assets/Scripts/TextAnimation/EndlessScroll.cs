using UnityEngine;

public class EndlessScroll : MonoBehaviour
{
    [Header("Objetos")]
    [SerializeField] private RectTransform[] objetos;

    [Header("Movimento")]
    [SerializeField] private float velocidade = 100f;
    [SerializeField] private Vector2 direcao = Vector2.left;

    [Header("Loop")]
    [SerializeField] private float distancia = 1920f;

    private void Update()
    {
        Vector2 movimento =
            direcao.normalized *
            velocidade *
            Time.unscaledDeltaTime;

        foreach (RectTransform objeto in objetos)
        {
            objeto.anchoredPosition += movimento;
        }

        VerificarLoop();
    }

    private void VerificarLoop()
    {
        for (int i = 0; i < objetos.Length; i++)
        {
            float distanciaMovida =
                Vector2.Dot(
                    objetos[i].anchoredPosition,
                    -direcao.normalized
                );

            if (distanciaMovida >= distancia)
            {
                RectTransform ultimo = objetos[0];

                for (int j = 1; j < objetos.Length; j++)
                {
                    if (Vector2.Dot(
                        objetos[j].anchoredPosition - objetos[i].anchoredPosition,
                        -direcao.normalized) < 0)
                    {
                        ultimo = objetos[j];
                    }
                }

                objetos[i].anchoredPosition =
                    ultimo.anchoredPosition -
                    direcao.normalized * distancia;
            }
        }
    }
}
