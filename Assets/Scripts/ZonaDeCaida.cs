using UnityEngine;
using System.Collections;

// Este script va en un Collider2D grande (Is Trigger) ubicado debajo de todo el mapa,
// fuera de la vista de la cámara. Cuando el jugador lo toca, significa que cayó fuera
// de cualquier plataforma, y lo reubicamos en un punto seguro.
public class ZonaDeCaida : MonoBehaviour
{
    [Header("Punto de reaparición")]
    public Transform puntoDeRespawn;

    [Header("Opcional: feedback")]
    public bool reiniciarVelocidad = true;

    [Header("Efecto de titileo al reaparecer")]
    public float duracionTitileo = 1.5f;
    public float intervaloTitileo = 0.1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (puntoDeRespawn != null)
            {
                collision.transform.position = puntoDeRespawn.position;
            }
            else
            {
                Debug.LogWarning("ZonaDeCaida: falta asignar un Punto De Respawn en el Inspector.");
            }

            if (reiniciarVelocidad && rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            SpriteRenderer sr = collision.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                StartCoroutine(TitilarSprite(sr));
            }

            Debug.Log("El jugador cayó fuera del mapa. Reapareció en el punto seguro.");
        }
    }

    private IEnumerator TitilarSprite(SpriteRenderer sr)
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionTitileo)
        {
            sr.enabled = !sr.enabled; // alterna visible/invisible
            yield return new WaitForSeconds(intervaloTitileo);
            tiempoTranscurrido += intervaloTitileo;
        }

        sr.enabled = true; // asegura que termine visible
    }
}