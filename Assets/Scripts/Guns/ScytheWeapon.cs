using UnityEngine;

public class ScytheWeapon : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject scythePrefab;
    [SerializeField] private AimingOrbit aimingOrbit;
    [SerializeField] private Transform player;

    [Header("Configurações")]
    [SerializeField] private float intervalo = 7f;

    private float contador;

    private void Update()
    {
        contador += Time.deltaTime;

        if (contador >= intervalo)
        {
            Atirar();
            contador = 0f;
        }
    }

    private void Atirar()
    {
        Vector2 direcao = aimingOrbit.Direcao;

        GameObject scythe = Instantiate(
            scythePrefab,
            player.position,
            Quaternion.identity
        );

        Scythe script = scythe.GetComponent<Scythe>();

        script.Lancar(direcao);
    }
}
