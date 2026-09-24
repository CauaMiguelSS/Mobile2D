using UnityEngine;
using UnityEngine.EventSystems;

public class TouchScreen : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private AimingOrbit _aimingOrbit;
    [SerializeField] private Camera _camera;

    private Vector2 _startPosition;
    private bool tocouNoChest;

    public void OnPointerDown(PointerEventData eventData)
    {
        _startPosition = eventData.position;

        tocouNoChest = VerificarChest(eventData.position);

        if (tocouNoChest)
            return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (tocouNoChest)
            return;

        Vector2 direcao = eventData.position - _startPosition;

        if (direcao == Vector2.zero)
            return;

        _aimingOrbit.SetDirection(direcao);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        tocouNoChest = false;
    }

    private bool VerificarChest(Vector2 posicaoTela)
    {
        Vector3 posicaoMundo =
            _camera.ScreenToWorldPoint(posicaoTela);

        posicaoMundo.z = 0f;

        Collider2D[] colisores =
            Physics2D.OverlapPointAll(posicaoMundo);

        foreach (Collider2D colisor in colisores)
        {
            WeaponChest chest =
                colisor.GetComponent<WeaponChest>();

            if (chest != null)
            {
                chest.Abrir();
                return true;
            }
        }

        return false;
    }
}
