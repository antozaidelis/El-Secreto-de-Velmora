using UnityEngine;

public class HuronAI : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 3f;
    public float radioDeteccion = 5f;
    public float distanciaMaximaDeCasa = 8f;
    public float margenFrenado = 0.5f;

    [Header("Referencias")]
    public Transform jugador;

    private Vector3 posicionInicial;
    private bool persiguiendo = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        // Si ya fue derrotado en esta sesión de juego, desaparece directamente
        if (GameManager.Instancia != null && GameManager.Instancia.huronDerrotado)
        {
            Destroy(gameObject);
            return;
        }

        posicionInicial = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) jugador = playerObj.transform;
        }
    }

    void Update()
    {
        if (jugador == null) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);
        float distanciaACasa = Vector3.Distance(transform.position, posicionInicial);

        // LÓGICA DE DETECCIÓN
        if (!persiguiendo && distanciaAlJugador <= radioDeteccion)
        {
            persiguiendo = true;
        }

        // LÓGICA DE PERSECUCIÓN
        if (persiguiendo)
        {
            if (distanciaACasa > distanciaMaximaDeCasa || distanciaAlJugador > radioDeteccion * 1.5f)
            {
                persiguiendo = false;
            }
            else
            {
                float distanciaX = Mathf.Abs(transform.position.x - jugador.position.x);

                if (distanciaX > margenFrenado)
                {
                    if (animator != null) animator.SetBool("isWalking", true);

                    Vector3 objectiveX = new Vector3(jugador.position.x, transform.position.y, transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, objectiveX, velocidad * Time.deltaTime);

                    if (jugador.position.x > transform.position.x)
                        spriteRenderer.flipX = false;
                    else
                        spriteRenderer.flipX = true;
                }
                else
                {
                    if (animator != null) animator.SetBool("isWalking", false);
                }
            }
        }
        else
        {
            // VOLVIENDO A CASA
            if (transform.position != posicionInicial)
            {
                if (animator != null) animator.SetBool("isWalking", true);

                transform.position = Vector3.MoveTowards(transform.position, posicionInicial, velocidad * 0.7f * Time.deltaTime);

                if (posicionInicial.x > transform.position.x)
                    spriteRenderer.flipX = false;
                else
                    spriteRenderer.flipX = true;
            }
            else
            {
                if (animator != null) animator.SetBool("isWalking", false);
            }
        }
    }

    // DETECCIÓN DEL CHOQUE E INICIO DE COMBATE
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("¡EL HURÓN TE ATRAPÓ! Intentando cargar sistema_combate...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("sistema_combate");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(posicionInicial == Vector3.zero ? transform.position : posicionInicial, distanciaMaximaDeCasa);
    }
}