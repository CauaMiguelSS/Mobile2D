
using UnityEngine;

public static class MelhoriasArmas
{
    public const string Pa = "Pa";
    public const string Tridente = "Tridente";
    public const string Foice = "Foice";
    public const string Projetil = "Projetil";

    public static readonly int[] Precos =
    {
        10, 20, 35, 50, 75,
        100, 140, 180, 230, 300
    };

    private static string Chave(string id)
    {
        return "NivelArma_" + id;
    }

    public static int Nivel(string id)
    {
        return PlayerPrefs.GetInt(Chave(id), 0);
    }

    public static float Multiplicador(string id)
    {
        return 1f + Nivel(id) * 0.1f;
    }

    public static int ProximoPreco(string id)
    {
        int nivel = Nivel(id);

        if (nivel >= 10)
            return 0;

        return Precos[nivel];
    }

    public static bool Melhorar(string id)
    {
        int nivel = Nivel(id);

        if (nivel >= 10)
            return false;

        int preco = ProximoPreco(id);

        if (!EconomiaJogo.Gastar(preco))
            return false;

        PlayerPrefs.SetInt(Chave(id), nivel + 1);
        PlayerPrefs.Save();

        return true;
    }
}
