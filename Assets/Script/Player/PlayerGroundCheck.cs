using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    private void Update()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        IsGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            checkRadius,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            checkRadius
        );
    }
}