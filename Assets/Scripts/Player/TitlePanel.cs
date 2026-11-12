
using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    [Header("Músicas")]
    [SerializeField] private AudioSource musicaTitulo;
    [SerializeField] private AudioSource musicaJogo;

    private void Start()
    {
        Time.timeScale = 0f;

        if (musicaTitulo != null)
        {
            musicaTitulo.loop = true;
            musicaTitulo.Play();
        }

        if (musicaJogo != null)
        {
            musicaJogo.loop = true;
            musicaJogo.Stop();
        }
    }

    public void IniciarJogo()
    {
        Time.timeScale = 1f;

        if (musicaTitulo != null)
            musicaTitulo.Stop();

        if (musicaJogo != null)
            musicaJogo.Play();

        gameObject.SetActive(false);
    }
}