using UnityEngine;
using System.Collections.Generic;

public class RecolectorIngredientes : MonoBehaviour
{
    [Header("Recolección")]
    public KeyCode teclaRecolectar = KeyCode.E;

    private List<GameObject> ingredientesCercanos = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ingrediente"))
        {
            if (!ingredientesCercanos.Contains(collision.gameObject))
                ingredientesCercanos.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ingrediente"))
        {
            ingredientesCercanos.Remove(collision.gameObject);
        }
    }

    void Update()
    {
        // Limpia referencias a ingredientes que ya no existen (por si fueron destruidos)
        ingredientesCercanos.RemoveAll(item => item == null);

        if (ingredientesCercanos.Count > 0 && Input.GetKeyDown(teclaRecolectar))
        {
            RecolectarMasCercano();
        }
    }

    private void RecolectarMasCercano()
    {
        GameObject masCercano = null;
        float distanciaMinima = float.MaxValue;

        foreach (GameObject ingrediente in ingredientesCercanos)
        {
            float distancia = Vector2.Distance(transform.position, ingrediente.transform.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                masCercano = ingrediente;
            }
        }

        if (masCercano != null)
        {
            string nombreLimpio = masCercano.name.Split('(')[0].Trim();

            if (GameManager.Instancia != null)
                GameManager.Instancia.AgregarIngrediente(nombreLimpio);

            ingredientesCercanos.Remove(masCercano);
            Destroy(masCercano);
        }
    }
}