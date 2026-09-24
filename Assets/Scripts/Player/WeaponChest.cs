using UnityEngine;

public class WeaponChest : MonoBehaviour
{
    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = FindFirstObjectByType<WeaponManager>();
    }

    public void Abrir()
    {
        if (weaponManager == null)
        {
            return;
        }

        string arma = weaponManager.DesbloquearArma();

        Destroy(gameObject);
    }
}