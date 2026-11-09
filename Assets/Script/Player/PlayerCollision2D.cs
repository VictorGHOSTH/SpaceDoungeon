
using UnityEngine;

public class PlayerCollision2D : MonoBehaviour
{
    [Header("Wall Detection")]
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.1f, 0.8f);

    [Header("Ceiling Detection")]
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Vector2 ceilingCheckSize = new Vector2(0.6f, 0.1f);

    [Header("Collision Settings")]
    [SerializeField] private LayerMask groundLayer;

    public bool IsTouchingWallLeft { get; private set; }
    public bool IsTouchingWallRight { get; private set; }
    public bool IsTouchingCeiling { get; private set; }

    private void Update()
    {
        CheckWalls();
        CheckCeiling();
    }

    private void CheckWalls()
    {
        IsTouchingWallLeft = Physics2D.OverlapBox(
            wallCheckLeft.position,
            wallCheckSize,
            0f,
            groundLayer
        );

        IsTouchingWallRight = Physics2D.OverlapBox(
            wallCheckRight.position,
            wallCheckSize,
            0f,
            groundLayer
        );
    }

    private void CheckCeiling()
    {
        IsTouchingCeiling = Physics2D.OverlapBox(
            ceilingCheck.position,
            ceilingCheckSize,
            0f,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (wallCheckLeft != null)
        {
            Gizmos.DrawWireCube(
                wallCheckLeft.position,
                wallCheckSize
            );
        }

        if (wallCheckRight != null)
        {
            Gizmos.DrawWireCube(
                wallCheckRight.position,
                wallCheckSize
            );
        }

        Gizmos.color = Color.yellow;

        if (ceilingCheck != null)
        {
            Gizmos.DrawWireCube(
                ceilingCheck.position,
                ceilingCheckSize
            );
        }
    }
}
