using UnityEngine;

public class PitchforkWeapon : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject pitchforkPrefab;
    [SerializeField] private AimingOrbit aimingOrbit;
    [SerializeField] private Transform player;

    private void Start()
    {
        LancarPitchfork();
    }

    private void LancarPitchfork()
    {
        GameObject pitchfork = Instantiate(
            pitchforkPrefab,
            player.position,
            Quaternion.identity
        );

        Pitchfork script = pitchfork.GetComponent<Pitchfork>();

        if (script != null)
        {
            script.Launch(aimingOrbit.Direcao);
        }
    }
}