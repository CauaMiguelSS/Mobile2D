
using System;
using UnityEngine;

public class AutoShoot : MonoBehaviour
{
    [Serializable]
    private class ConfiguracaoArma
    {
        public WeaponBuff buff;
        public GameObject projectilePrefab;
        public Transform pontoDeTiro;
        public AimingOrbit aimingOrbit;

        [Min(0.01f)]
        public float intervaloTiro = 0.5f;

        [NonSerialized] public float contador;
    }

    [Header("Armas")]
    [SerializeField] private ConfiguracaoArma[] armas;

    private void Update()
    {
        if (armas == null)
            return;

        foreach (ConfiguracaoArma arma in armas)
        {
            if (arma == null || arma.buff == null)
                continue;

            if (!arma.buff.gameObject.activeInHierarchy)
                continue;

            if (arma.projectilePrefab == null ||
                arma.pontoDeTiro == null ||
                arma.aimingOrbit == null)
                continue;

            arma.contador += Time.deltaTime;

            float intervaloAtual =
                arma.intervaloTiro / arma.buff.Intervalo;

            if (arma.contador >= intervaloAtual)
            {
                Atirar(arma);
                arma.contador = 0f;
            }
        }
    }

    private void Atirar(ConfiguracaoArma arma)
    {
        GameObject projetil = Instantiate(
            arma.projectilePrefab,
            arma.pontoDeTiro.position,
            Quaternion.identity
        );

        Projectile script = projetil.GetComponent<Projectile>();

        if (script == null)
        {
            Destroy(projetil);
            return;
        }

        script.ConfigurarClone();
        script.SetDirection(arma.aimingOrbit.Direcao);
        script.AplicarBuff(arma.buff);
    }
}
