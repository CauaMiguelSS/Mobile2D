using UnityEngine;
using UnityEngine.EventSystems;

public class AbrirPainel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject painel;

    public void OnPointerClick(PointerEventData eventData)
    {
        painel.SetActive(!painel.activeSelf);
    }
}
