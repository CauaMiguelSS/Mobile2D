using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 0f;
    }

    public void IniciarJogo()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
