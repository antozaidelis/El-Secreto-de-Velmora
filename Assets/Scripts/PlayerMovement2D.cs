using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        if (horizontal > 0)
            transform.localScale = new Vector3(0.278f, 0.278f, 1);
        else if (horizontal < 0)
            transform.localScale = new Vector3(-0.278f, 0.278f, 1);

        animator.SetBool("isWalking", horizontal != 0);
    }
}