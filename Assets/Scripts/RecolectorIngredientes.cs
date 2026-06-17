using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecolectorIngredientes : MonoBehaviour
{
    [Header("Configuración del Inventario")]
    public int capacityMaxima = 12;
    public List<string> mochilaDeIngredientes = new List<string>();

    [Header("UI")]
    public InventarioUI inventarioUI;

    [Header("Iconos")]
    public List<Sprite> iconosDisponibles;
    public List<string> nombresDeIconos;

    private Dictionary<string, Sprite> mapaIconos = new Dictionary<string, Sprite>();

    void Start()
    {
        for (int i = 0; i < nombresDeIconos.Count; i++)
        {
            if (i < iconosDisponibles.Count)
                mapaIconos[nombresDeIconos[i]] = iconosDisponibles[i];
        }
        foreach (var key in mapaIconos.Keys)
            Debug.Log("Clave en mapa: '" + key + "'");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ingrediente"))
        {
            if (mochilaDeIngredientes.Count < capacityMaxima)
            {
                string nuevoIngrediente = collision.gameObject.name.Split('(')[0].Trim();
                mochilaDeIngredientes.Add(nuevoIngrediente);
                Debug.Log("¡Guardado en mochila: " + nuevoIngrediente + "!");
                ActualizarUI();
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.Log("¡Mochila llena!");
            }
        }
    }

    void ActualizarUI()
    {
        List<Sprite> iconos = new List<Sprite>();
        foreach (string nombre in mochilaDeIngredientes)
        {
            Debug.Log("Buscando: '" + nombre + "'");
            if (mapaIconos.ContainsKey(nombre))
            {
                Debug.Log("Encontrado!");
                iconos.Add(mapaIconos[nombre]);
            }
            else
            {
                Debug.Log("NO encontrado para: '" + nombre + "'");
            }
        }
        inventarioUI.ActualizarUI(iconos);
    }
}