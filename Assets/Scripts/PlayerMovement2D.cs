using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Sonido de pasos")]
    public AudioSource audioPasos;
    public AudioClip clipPasos;

    [Header("Sonido de idle (quieto)")]
    public AudioSource audioIdle;
    public AudioClip clipIdle;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            animator.SetBool("isJumping", true);
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        if (horizontal > 0)
            transform.localScale = new Vector3(0.278f, 0.278f, 1);
        else if (horizontal < 0)
            transform.localScale = new Vector3(-0.278f, 0.278f, 1);

        bool estaCaminando = horizontal != 0 && isGrounded;
        bool estaQuieto = horizontal == 0 && isGrounded;

        animator.SetBool("isWalking", estaCaminando);

        ActualizarSonidoPasos(estaCaminando);
        ActualizarSonidoIdle(estaQuieto);
    }

    private void ActualizarSonidoPasos(bool estaCaminando)
    {
        if (audioPasos == null || clipPasos == null) return;

        if (estaCaminando && !audioPasos.isPlaying)
        {
            audioPasos.clip = clipPasos;
            audioPasos.loop = true;
            audioPasos.Play();
        }
        else if (!estaCaminando && audioPasos.isPlaying)
        {
            audioPasos.Stop();
        }
    }

    private void ActualizarSonidoIdle(bool estaQuieto)
    {
        if (audioIdle == null || clipIdle == null) return;

        if (estaQuieto && !audioIdle.isPlaying)
        {
            audioIdle.clip = clipIdle;
            audioIdle.loop = true;
            audioIdle.Play();
        }
        else if (!estaQuieto && audioIdle.isPlaying)
        {
            audioIdle.Stop();
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }
}