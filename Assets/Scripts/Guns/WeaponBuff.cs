
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
    [Header("Informações da arma")]
    [SerializeField] private string nomeArma = "Arma";
    [SerializeField] private bool possuiIntervalo = true;

    private float dano = 1f;
    private float velocidade = 1f;
    private float tamanho = 1f;
    private float intervalo = 1f;

    public string NomeArma => nomeArma;
    public bool PossuiIntervalo => possuiIntervalo;

    public float Dano => dano;
    public float Velocidade => velocidade;
    public float Tamanho => tamanho;
    public float Intervalo => intervalo;

    public void AplicarBuff(TipoBuff tipo, float porcentagem)
    {
        float multiplicador = 1f + porcentagem;

        switch (tipo)
        {
            case TipoBuff.Dano:
                dano *= multiplicador;
                break;

            case TipoBuff.Velocidade:
                velocidade *= multiplicador;
                break;

            case TipoBuff.Tamanho:
                tamanho *= multiplicador;
                break;

            case TipoBuff.Intervalo:
                if (possuiIntervalo)
                    intervalo *= multiplicador;
                break;
        }

        Debug.Log(
            $"Buff aplicado em {nomeArma}: {tipo} +{porcentagem * 100f:F0}%"
        );
    }
}
