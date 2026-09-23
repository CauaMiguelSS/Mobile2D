using UnityEngine;

public class AutoShoot : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform pontoDeTiro;
    [SerializeField] private AimingOrbit aimingOrbit;

    [Header("Configurações")]
    [SerializeField] private float intervaloTiro = 0.5f;

    private float contador;

    private void Update()
    {
        contador += Time.deltaTime;

        if (contador >= intervaloTiro)
        {
            Atirar();
            contador = 0f;
        }
    }

    private void Atirar()
    {
        GameObject projetil = Instantiate(projectilePrefab, pontoDeTiro.position, Quaternion.identity);

        projetil.GetComponent<Projectile>().SetDirection(aimingOrbit.Direcao);
    }
}
