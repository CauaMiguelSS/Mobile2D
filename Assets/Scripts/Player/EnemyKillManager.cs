using UnityEngine;

public class EnemyKillManager : MonoBehaviour
{
    [Header("Baú")]
    [SerializeField] private GameObject bauPrefab;
    [SerializeField] private int mortesParaBaú = 10;
    [SerializeField] private int maximoDeBaus = 3;

    private int inimigosMortos;
    private int bausCriados;

    public void InimigoMorreu(Vector3 posicao)
    {
        inimigosMortos++;

        if (inimigosMortos % mortesParaBaú == 0 &&
            bausCriados < maximoDeBaus)
        {
            CriarBau(posicao);
        }
    }

    private void CriarBau(Vector3 posicao)
    {
        Instantiate(bauPrefab, posicao, Quaternion.identity);

        bausCriados++;
    }
}
