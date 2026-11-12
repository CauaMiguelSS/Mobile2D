
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
        public string idMelhoria = "Projetil";

        [Header("Áudio")]
        public AudioClip somDisparo;

        [Min(0.01f)]
        public float intervaloTiro = 0.5f;

        [NonSerialized] public float contador;
    }

    [Header("Armas")]
    [SerializeField] private ConfiguracaoArma[] armas;

    [Header("Áudio")]
    [SerializeField] private AudioSource audioSource;
    public AudioClip somDisparo;

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
        if (audioSource != null && arma.somDisparo != null)
        {
            audioSource.PlayOneShot(arma.somDisparo);
        }

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
        script.AplicarMelhoriaLoja(arma.idMelhoria);
    }
}
