
using UnityEngine;

public class ScytheWeapon : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject scythePrefab;
    [SerializeField] private AimingOrbit aimingOrbit;
    [SerializeField] private Transform player;
    [SerializeField] private WeaponBuff buff;

    [Header("Configurações")]
    [SerializeField] private float intervalo = 7f;

    private float contador;

    private void Awake()
    {
        if (buff == null)
            buff = GetComponent<WeaponBuff>();

        if (buff == null)
            Debug.LogError("ScytheWeapon: WeaponBuff não encontrado!", gameObject);
    }

    private void Update()
    {
        if (buff == null)
            return;

        contador += Time.deltaTime;

        if (contador >= intervalo / buff.Intervalo)
        {
            Atirar();
            contador = 0f;
        }
    }

    private void Atirar()
    {
        if (scythePrefab == null || player == null || aimingOrbit == null)
        {
            Debug.LogError("ScytheWeapon: faltam referências no Inspector!", gameObject);
            return;
        }

        GameObject scythe = Instantiate(
            scythePrefab,
            player.position,
            Quaternion.identity
        );

        Scythe script = scythe.GetComponent<Scythe>();

        if (script != null)
        {
            script.Lancar(aimingOrbit.Direcao);
            script.AplicarBuff(buff);
        }
        else
        {
            Debug.LogError("O prefab da foice não possui o script Scythe!");
            Destroy(scythe);
        }
    }
}
