using UnityEngine;
using System.Collections.Generic; // ¡Esto nos permite usar Listas!

public class RecolectorIngredientes : MonoBehaviour
{
    [Header("Configuración del Inventario")]
    public int capacityMaxima = 12;

    // Esta es nuestra mochila invisible. Aquí se guardan los nombres.
    public List<string> mochilaDeIngredientes = new List<string>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ingrediente"))
        {
            // Revisamos si el tamaño de la lista es menor a 12
            if (mochilaDeIngredientes.Count < capacityMaxima)
            {
                // Agarrás el nombre del objeto tal cual se llama en Unity
                string nuevoIngrediente = collision.gameObject.name;

                // ¡Lo metemos adentro de la mochila!
                mochilaDeIngredientes.Add(nuevoIngrediente);

                Debug.Log("¡Guardado en mochila: " + nuevoIngrediente + "!");

                // Mostrar en consola todo lo que tenemos actualmente
                MostrarMochilaEnConsola();

                // Destruimos el pimiento del suelo
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.Log("¡Mochila llena! No puedes cargar más de " + capacityMaxima + " ingredientes.");
            }
        }
    }

    // Una función ayudante para revisar qué tenemos adentro desde Unity
    void MostrarMochilaEnConsola()
    {
        string contenido = "Contenido actual de la mochila: ";

        foreach (string ingrediente in mochilaDeIngredientes)
        {
            contenido += "[" + ingrediente + "] ";
        }

        Debug.Log(contenido);
    }
}
    