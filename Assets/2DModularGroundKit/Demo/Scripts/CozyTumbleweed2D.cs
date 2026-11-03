using UnityEngine;
namespace Timbeaux.ModularGroundKit { 
public class CozyTumbleweed2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector2 moveDirection = Vector2.right;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 240f;

    private void Update()
    {
        Vector2 direction =
            moveDirection.normalized;

        transform.position +=
            (Vector3)(
                direction *
                moveSpeed *
                Time.deltaTime
            );

        transform.Rotate(
            0f,
            0f,
            -rotationSpeed *
            Time.deltaTime
        );
    }
}}