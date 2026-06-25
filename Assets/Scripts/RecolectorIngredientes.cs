using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RecolectorIngredientes : MonoBehaviour
{
    [Header("Recolección")]
    public KeyCode teclaRecolectar = KeyCode.E;

    [Header("Indicador visual de interacción")]
    public TextMeshPro indicadorInteraccion;
    public Vector3 offsetIndicador = new Vector3(0f, 1f, 0f);

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
        ingredientesCercanos.RemoveAll(item => item == null);

        GameObject masCercano = ObtenerMasCercano();

        ActualizarIndicador(masCercano);

        if (masCercano != null && Input.GetKeyDown(teclaRecolectar))
        {
            RecolectarIngrediente(masCercano);
        }
    }

    private GameObject ObtenerMasCercano()
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

        return masCercano;
    }

    private void ActualizarIndicador(GameObject objetivo)
    {
        if (indicadorInteraccion == null) return;

        if (objetivo != null)
        {
            indicadorInteraccion.gameObject.SetActive(true);
            indicadorInteraccion.transform.position = objetivo.transform.position + offsetIndicador;
        }
        else
        {
            indicadorInteraccion.gameObject.SetActive(false);
        }
    }

    private void RecolectarIngrediente(GameObject ingrediente)
    {
        string nombreLimpio = ingrediente.name.Split('(')[0].Trim();

        if (GameManager.Instancia != null)
            GameManager.Instancia.AgregarIngrediente(nombreLimpio);

        ingredientesCercanos.Remove(ingrediente);
        Destroy(ingrediente);
    }
}