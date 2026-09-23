using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponChest : MonoBehaviour, IPointerClickHandler
{
    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = FindFirstObjectByType<WeaponManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (weaponManager == null)
        {
            Debug.LogError("WeaponManager não encontrado!");
            return;
        }

        string arma = weaponManager.DesbloquearArma();

        Debug.Log("Conseguiu a arma: " + arma);

        Destroy(gameObject);
    }
}