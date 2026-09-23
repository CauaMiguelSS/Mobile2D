using UnityEngine;

public class WeaponChest : MonoBehaviour
{
    [SerializeField] private WeaponManager weaponManager;

    private bool aberto;

    private void Start()
    {
        if (weaponManager == null)
            weaponManager = FindFirstObjectByType<WeaponManager>();
    }

    private void OnMouseDown()
    {
        if (aberto)
            return;

        aberto = true;

        string arma = weaponManager.DesbloquearArma();

        Debug.Log("Conseguiu a arma: " + arma);

        Destroy(gameObject);
    }
}
