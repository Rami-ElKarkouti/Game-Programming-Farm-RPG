using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float climbSpeed = 3f;
    private const float collisionSkin = 0.02f;

    [Header("Map Bounds")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [Header("Player Edge Padding")]
    [SerializeField] private float paddingX = 0.3f;
    [SerializeField] private float paddingY = 0.3f;

    [Header("Bridge 2 Blocker")]
    [SerializeField] private Collider2D bridge2Collider;
    [SerializeField] private Renderer bridge2Renderer;
    [SerializeField] private Tilemap bridge2Tilemap;
    [SerializeField] private float bridgeBlockPadding = 0.05f;

    [Header("Player Visuals")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer hairRenderer;

    [SerializeField] private Collider2D playerCollider;

    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

    private Rigidbody2D rb;
    private Rigidbody rb3d;
    private Animator animator;
    private Vector2 movement;
    private readonly RaycastHit2D[] movementHits = new RaycastHit2D[16];
    private readonly Collider2D[] nextStepHits = new Collider2D[32];
    private bool onLadder = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb3d = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }

        CacheBridgeReference();

        if (rb != null)
        {
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        EnsureNo2DGravity();
        EnsureNo3DGravity();
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        UpdatePlayerVisualDirection();

        if (animator != null)
        {
            animator.SetBool(IsWalkingHash, movement.sqrMagnitude > 0f);
        }
    }

    private void UpdatePlayerVisualDirection()
    {
        if (visualRoot == null)
        {
            return;
        }

        if (movement == Vector2.zero)
        {
            return;
        }

        // Horizontal movement
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            visualRoot.localRotation = Quaternion.identity;

            bool facingLeft = movement.x < 0f;

            if (bodyRenderer != null)
            {
                bodyRenderer.flipX = facingLeft;
            }

            if (hairRenderer != null)
            {
                hairRenderer.flipX = facingLeft;
            }

            return;
        }


        if (bodyRenderer != null)
        {
            bodyRenderer.flipX = false;
        }

        if (hairRenderer != null)
        {
            hairRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        EnsureNo2DGravity();
        EnsureNo3DGravity();

        if (rb == null)
        {
            return;
        }

        Vector2 velocity = Vector2.zero;

        if (movement == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (onLadder)
        {
            rb.gravityScale = 0f;
            velocity = new Vector2(movement.x * speed, movement.y * climbSpeed);
        }
        else
        {
            rb.gravityScale = 0f;
            velocity = movement * speed;
        }

        Vector2 currentPosition = rb.position;
        Vector2 nextPosition = currentPosition + velocity * Time.fixedDeltaTime;

        float minClampY = minY + paddingY;
        float maxClampY = maxY - paddingY;
        float bridgeLimitY = GetBridgeBlockerMaxY();

        if (!float.IsNaN(bridgeLimitY))
        {
            maxClampY = Mathf.Min(maxClampY, bridgeLimitY);
        }

        if (maxClampY < minClampY)
        {
            maxClampY = minClampY;
        }

        nextPosition = ClampToMapBounds(nextPosition, minClampY, maxClampY);
        nextPosition = ResolveBlockedMovement(currentPosition, nextPosition, minClampY, maxClampY);

        rb.MovePosition(nextPosition);
    }

    private Vector2 ClampToMapBounds(Vector2 position, float minClampY, float maxClampY)
    {
        position.x = Mathf.Clamp(position.x, minX + paddingX, maxX - paddingX);
        position.y = Mathf.Clamp(position.y, minClampY, maxClampY);
        return position;
    }

    private Vector2 ResolveBlockedMovement(Vector2 currentPosition, Vector2 nextPosition, float minClampY, float maxClampY)
    {
        if (!IsBlocked(currentPosition, nextPosition))
        {
            return nextPosition;
        }

        Vector2 horizontalPosition = ClampToMapBounds(new Vector2(nextPosition.x, currentPosition.y), minClampY, maxClampY);
        if (!IsBlocked(currentPosition, horizontalPosition))
        {
            return horizontalPosition;
        }

        Vector2 verticalPosition = ClampToMapBounds(new Vector2(currentPosition.x, nextPosition.y), minClampY, maxClampY);
        if (!IsBlocked(currentPosition, verticalPosition))
        {
            return verticalPosition;
        }

        return currentPosition;
    }

    private bool IsBlocked(Vector2 fromPosition, Vector2 toPosition)
    {
        if (playerCollider == null)
        {
            return false;
        }

        if (IsOnBridge2Tile(toPosition))
        {
            return true;
        }

        Vector2 delta = toPosition - fromPosition;
        float distance = delta.magnitude;
        if (distance <= Mathf.Epsilon)
        {
            return false;
        }

        if (HasBlockingObjectAtNextStep(fromPosition, toPosition))
        {
            return true;
        }

        ContactFilter2D filter = new ContactFilter2D
        {
            useTriggers = false,
            useLayerMask = false
        };

        int hitCount = playerCollider.Cast(delta.normalized, filter, movementHits, distance + collisionSkin);
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = movementHits[i].collider;
            if (hitCollider != null && BlocksPlayerMovement(hitCollider))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasBlockingObjectAtNextStep(Vector2 fromPosition, Vector2 toPosition)
    {
        if (playerCollider == null)
        {
            return false;
        }

        Bounds playerBounds = playerCollider.bounds;
        Vector2 delta = toPosition - fromPosition;
        Vector2 probeCenter = (Vector2)playerBounds.center + delta;
        Vector2 probeSize = new Vector2(playerBounds.size.x, playerBounds.size.y);

        int hitCount = Physics2D.OverlapBoxNonAlloc(probeCenter, probeSize, 0f, nextStepHits);
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = nextStepHits[i];
            if (hitCollider == null || hitCollider == playerCollider)
            {
                continue;
            }

            if (BlocksPlayerMovement(hitCollider))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsOnBridge2Tile(Vector2 playerPosition)
    {
        if (bridge2Tilemap == null)
        {
            CacheBridgeReference();
        }

        if (bridge2Tilemap == null)
        {
            return false;
        }

        Vector2 halfExtents = Vector2.one * 0.1f;
        if (playerCollider != null)
        {
            halfExtents = playerCollider.bounds.extents;
        }

        return HasBridgeTileAt(playerPosition)
            || HasBridgeTileAt(playerPosition + new Vector2(halfExtents.x, halfExtents.y))
            || HasBridgeTileAt(playerPosition + new Vector2(-halfExtents.x, halfExtents.y))
            || HasBridgeTileAt(playerPosition + new Vector2(halfExtents.x, -halfExtents.y))
            || HasBridgeTileAt(playerPosition + new Vector2(-halfExtents.x, -halfExtents.y));
    }

    private bool HasBridgeTileAt(Vector2 worldPosition)
    {
        Vector3Int cellPosition = bridge2Tilemap.WorldToCell(worldPosition);
        return bridge2Tilemap.HasTile(cellPosition);
    }

    private bool BlocksPlayerMovement(Collider2D hitCollider)
    {
        if (hitCollider.isTrigger)
        {
            return false;
        }

        string objectName = hitCollider.gameObject.name;

        return NameOrParentContains(hitCollider.transform, "Rock")
            || NameOrParentContains(hitCollider.transform, "Tree")
            || NameOrParentContains(hitCollider.transform, "Shop")
            || objectName.Contains("Water")
            || objectName.Contains("Bridge2Blocker")
            || IsHouseBodyCollider(hitCollider);
    }
    private bool IsHouseBodyCollider(Collider2D hitCollider)
    {
        Transform hitTransform = hitCollider.transform;
        string objectName = hitTransform.name;

        if (objectName.Contains("Door") || objectName.Contains("InteriorTrigger"))
        {
            return false;
        }

        while (hitTransform != null)
        {
            if (hitTransform.name.Contains("House"))
            {
                return !HouseDoorEnter.IsPlayerInsideHouse(transform, hitTransform);
            }

            hitTransform = hitTransform.parent;
        }

        return false;
    }

    private bool NameOrParentContains(Transform hitTransform, string namePart)
    {
        while (hitTransform != null)
        {
            if (hitTransform.name.Contains(namePart))
            {
                return true;
            }

            hitTransform = hitTransform.parent;
        }

        return false;
    }

    private void EnsureNo2DGravity()
    {
        if (rb == null)
        {
            return;
        }

        if (rb.bodyType != RigidbodyType2D.Kinematic)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (rb.gravityScale != 0f)
        {
            rb.gravityScale = 0f;
        }
    }

    private void EnsureNo3DGravity()
    {
        if (rb3d == null)
        {
            return;
        }

        if (rb3d.useGravity)
        {
            rb3d.useGravity = false;
        }

        if (!rb3d.isKinematic)
        {
            rb3d.isKinematic = true;
        }

        if (rb3d.constraints != RigidbodyConstraints.FreezeRotation)
        {
            rb3d.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void CacheBridgeReference()
    {
        if (bridge2Tilemap != null && (bridge2Collider != null || bridge2Renderer != null))
        {
            return;
        }

        GameObject bridge = GameObject.Find("Bridge 2");
        if (bridge == null)
        {
            return;
        }

        if (bridge2Tilemap == null)
        {
            bridge2Tilemap = bridge.GetComponent<Tilemap>();
        }

        if (bridge2Collider == null)
        {
            bridge2Collider = bridge.GetComponent<Collider2D>();
        }

        if (bridge2Collider == null)
        {
            bridge2Renderer = bridge.GetComponent<Renderer>();
        }
    }

    private float GetBridgeBlockerMaxY()
    {
        if (bridge2Collider == null && bridge2Renderer == null)
        {
            CacheBridgeReference();
        }

        float playerHalfHeight = 0f;
        if (playerCollider != null)
        {
            playerHalfHeight = playerCollider.bounds.extents.y;
        }

        if (bridge2Collider != null)
        {
            return bridge2Collider.bounds.min.y - playerHalfHeight - bridgeBlockPadding;
        }

        if (bridge2Renderer != null)
        {
            return bridge2Renderer.bounds.min.y - playerHalfHeight - bridgeBlockPadding;
        }

        return float.NaN;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            onLadder = true;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            onLadder = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
