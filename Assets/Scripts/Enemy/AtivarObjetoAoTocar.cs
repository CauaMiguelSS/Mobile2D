
using UnityEngine;

public class AtivarObjetoAoTocar : MonoBehaviour
{
    [SerializeField] private GameObject objetoParaAtivar;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            objetoParaAtivar.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            objetoParaAtivar.SetActive(false);
        }
    }
}

