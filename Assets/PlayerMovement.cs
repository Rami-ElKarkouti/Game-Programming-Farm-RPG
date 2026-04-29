using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float climbSpeed = 3f;

    [Header("Map Bounds")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [Header("Player Edge Padding")]
    [SerializeField] private float paddingX = 0.3f;
    [SerializeField] private float paddingY = 0.3f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool onLadder = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    private void FixedUpdate()
    {
        Vector2 velocity;

        if (movement == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (onLadder)
        {
            rb.gravityScale = 0f;
            velocity = new Vector2(movement.x * speed, movement.y * climbSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
            velocity = movement * speed;
        }

        Vector2 nextPosition = rb.position + velocity * Time.fixedDeltaTime;

        nextPosition.x = Mathf.Clamp(nextPosition.x, minX + paddingX, maxX - paddingX);
        nextPosition.y = Mathf.Clamp(nextPosition.y, minY + paddingY, maxY - paddingY);

        rb.MovePosition(nextPosition);
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