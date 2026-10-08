using UnityEngine;

public class AutoShoot : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform pontoDeTiro;
    [SerializeField] private AimingOrbit aimingOrbit;
    [SerializeField] private WeaponBuff buff;

    [Header("Configurações")]
    [SerializeField] private float intervaloTiro = 0.5f;

    private float contador;

    private void Start()
    {
        if (buff == null)
        {
            buff = GetComponent<WeaponBuff>();
        }

        if (buff == null)
        {
            Debug.LogError(
                "WeaponBuff não foi encontrado no Bullet!"
            );
        }
    }

    private void Update()
    {
        if (buff == null)
            return;

        contador += Time.deltaTime;

        float intervaloAtual =
            intervaloTiro / buff.Intervalo;

        if (contador >= intervaloAtual)
        {
            Atirar();
            contador = 0f;
        }
    }

    private void Atirar()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError(
                "Projectile Prefab não foi configurado!"
            );
            return;
        }

        GameObject projetil = Instantiate(
            projectilePrefab,
            pontoDeTiro.position,
            Quaternion.identity
        );

        Projectile script =
            projetil.GetComponent<Projectile>();

        if (script == null)
        {
            Debug.LogError(
                "O prefab do Projectile não possui o script Projectile!"
            );

            return;
        }

        script.SetDirection(
            aimingOrbit.Direcao
        );

        script.AplicarBuff(buff);
    }
}