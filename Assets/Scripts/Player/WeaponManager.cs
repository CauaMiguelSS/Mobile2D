using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Armas")]
    [SerializeField] private List<GameObject> armas;

    private void Awake()
    {
        for (int i = 0; i < armas.Count; i++)
        {
            if (armas[i] != null)
            {
                armas[i].SetActive(i == 0);
            }
        }
    }

    public string DesbloquearArma()
    {
        List<GameObject> bloqueadas = new List<GameObject>();

        foreach (GameObject arma in armas)
        {
            if (arma != null && !arma.activeSelf)
            {
                bloqueadas.Add(arma);
            }
        }

        GameObject armaEscolhida = bloqueadas[Random.Range(0, bloqueadas.Count)];

        armaEscolhida.SetActive(true);

        return armaEscolhida.name;
    }
}