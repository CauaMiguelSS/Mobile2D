using UnityEngine;

public static class EconomiaJogo
{
    private const string CHAVE = "MoedasJogador";

    public static int Saldo
    {
        get { return PlayerPrefs.GetInt(CHAVE, 0); }
    }

    public static void Adicionar(int quantidade)
    {
        PlayerPrefs.SetInt(CHAVE, Saldo + quantidade);
        PlayerPrefs.Save();
    }

    public static bool Gastar(int quantidade)
    {
        if (Saldo < quantidade)
            return false;

        PlayerPrefs.SetInt(CHAVE, Saldo - quantidade);
        PlayerPrefs.Save();

        return true;
    }
}
