using UnityEngine;

public class WeaponBuff : MonoBehaviour
{
    public float danoMultiplier = 1f;
    public float velocidadeMultiplier = 1f;
    public float tamanhoMultiplier = 1f;
    public float intervaloMultiplier = 1f;

    public void AplicarBuff(string tipo, float porcentagem)
    {
        float multiplicador = 1f + porcentagem;

        switch (tipo)
        {
            case "Dano":
                danoMultiplier *= multiplicador;
                break;

            case "Velocidade":
                velocidadeMultiplier *= multiplicador;
                break;

            case "Tamanho":
                tamanhoMultiplier *= multiplicador;
                break;

            case "Intervalo":
                intervaloMultiplier /= multiplicador;
                break;
        }
    }
}