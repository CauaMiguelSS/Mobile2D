using UnityEngine;
using TMPro;

public class BuffCard : MonoBehaviour
{
    [SerializeField] private TMP_Text texto;

    private WeaponBuff weaponBuff;
    private string tipo;
    private float porcentagem;

    public void Configurar(
        WeaponBuff arma,
        string tipoBuff,
        float valor
    )
    {
        weaponBuff = arma;
        tipo = tipoBuff;
        porcentagem = valor;

        texto.text =
            arma.gameObject.name +
            "\n" +
            tipoBuff +
            " +" +
            (valor * 100f).ToString("0") +
            "%";
    }

    public void Escolher()
    {
        weaponBuff.AplicarBuff(
            tipo,
            porcentagem
        );

        RandomEventManager evento =
            FindFirstObjectByType<RandomEventManager>();

        Time.timeScale = 1f;

        evento.FecharEvento();
    }
}