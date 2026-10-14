using UnityEngine;

public enum TipoBuff
{
    Dano,
    Velocidade,
    Tamanho,
    Intervalo
}

public class WeaponBuff : MonoBehaviour
{
    [Header("Informações")]
    [SerializeField] private string nomeArma = "Arma";
    [SerializeField] private bool possuiIntervalo;

    private float danoMultiplier = 1f;
    private float velocidadeMultiplier = 1f;
    private float tamanhoMultiplier = 1f;
    private float intervaloMultiplier = 1f;

    public string NomeArma => nomeArma;
    public float Dano => danoMultiplier;
    public float Velocidade => velocidadeMultiplier;
    public float Tamanho => tamanhoMultiplier;
    public float Intervalo => intervaloMultiplier;
    public bool PossuiIntervalo => possuiIntervalo;

    public void AplicarBuff(TipoBuff tipo, float porcentagem)
    {
        float multiplicador = 1f + porcentagem;

        switch (tipo)
        {
            case TipoBuff.Dano:
                danoMultiplier *= multiplicador;
                break;

            case TipoBuff.Velocidade:
                velocidadeMultiplier *= multiplicador;
                break;

            case TipoBuff.Tamanho:
                tamanhoMultiplier *= multiplicador;
                break;

            case TipoBuff.Intervalo:
                intervaloMultiplier *= multiplicador;
                break;
        }

        Debug.Log(
            $"Arma: {nomeArma} | " +
            $"Dano: {danoMultiplier:F2} | " +
            $"Velocidade: {velocidadeMultiplier:F2} | " +
            $"Tamanho: {tamanhoMultiplier:F2} | " +
            $"Intervalo: {intervaloMultiplier:F2}"
        );
    }
}