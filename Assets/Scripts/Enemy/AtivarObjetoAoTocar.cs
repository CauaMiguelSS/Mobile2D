
using UnityEngine;

public class AtivarObjetoAoTocar : MonoBehaviour
{
    [SerializeField] private GameObject objetoParaAtivar;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            objetoParaAtivar.SetActive(true);
        }
    }
}

