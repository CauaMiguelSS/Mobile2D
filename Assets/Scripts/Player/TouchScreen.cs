using UnityEngine;
using UnityEngine.EventSystems;

public class TouchScreen : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private AimingOrbit _aimingOrbit;

    private Vector2 _startPosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        _startPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direcao = eventData.position - _startPosition;

        if (direcao == Vector2.zero)
            return;

        _aimingOrbit.SetDirection(direcao);
    }

    public void OnPointerUp(PointerEventData eventData){}
}
