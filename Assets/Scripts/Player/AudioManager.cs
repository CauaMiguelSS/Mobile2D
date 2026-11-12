
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip somImpacto;

    private void Awake()
    {
        Instance = this;
    }

    public void TocarImpacto()
    {
        if (somImpacto != null && audioSource != null)
            audioSource.PlayOneShot(somImpacto);
    }
}
