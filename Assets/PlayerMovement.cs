using UnityEngine;

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
    [SerializeField] private float bridgeBlockPadding = 0.05f;

    [SerializeField] private Collider2D playerCollider;

    private Rigidbody2D rb;
    private Rigidbody rb3d;
    private Vector2 movement;
    private readonly RaycastHit2D[] movementHits = new RaycastHit2D[16];
    private bool onLadder = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb3d = GetComponent<Rigidbody>();

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

        Vector2 delta = toPosition - fromPosition;
        float distance = delta.magnitude;
        if (distance <= Mathf.Epsilon)
        {
            return false;
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

    private bool BlocksPlayerMovement(Collider2D hitCollider)
    {
        string objectName = hitCollider.gameObject.name;
        return objectName.Contains("Rock")
            || objectName.Contains("Water")
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
        if (bridge2Collider != null || bridge2Renderer != null)
        {
            return;
        }

        GameObject bridge = GameObject.Find("Bridge 2");
        if (bridge == null)
        {
            return;
        }

        bridge2Collider = bridge.GetComponent<Collider2D>();
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
