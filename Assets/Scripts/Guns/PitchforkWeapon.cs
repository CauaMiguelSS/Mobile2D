using UnityEngine;

public class PitchforkWeapon : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject pitchforkPrefab;
    [SerializeField] private AimingOrbit aimingOrbit;
    [SerializeField] private Transform player;

    private WeaponBuff buff;

    private void Start()
    {
        buff = GetComponent<WeaponBuff>();

        if (buff == null)
        {
            Debug.LogError("WeaponBuff não encontrado no PitchFork!");

            return;
        }

        LancarPitchfork();
    }

    private void LancarPitchfork()
    {
        GameObject pitchfork = Instantiate(
            pitchforkPrefab,
            player.position,
            Quaternion.identity
        );

        Pitchfork script =
            pitchfork.GetComponent<Pitchfork>();

        if (script != null)
        {
            script.Launch(
                aimingOrbit.Direcao,
                buff
            );
        }
        else
        {
            Debug.LogError("O prefab PitchFork não possui o script Pitchfork!");
        }
    }
}