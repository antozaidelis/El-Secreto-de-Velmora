using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventarioUI : MonoBehaviour
{

    [Header("Referencias")]
    public GameObject panelMochila;
    public List<Image> slots;
    public List<TextMeshProUGUI> textosCantidad;

    [Header("Recetas")]
    public GameObject seccionRecetas;

    private bool mochilaAbierta = false;
    private bool recetasAbiertas = false;

    void Start()
    {
        if (seccionRecetas != null)
            seccionRecetas.SetActive(false);

        // Iniciamos la corrutina para esperar un instante antes de dibujar
        StartCoroutine(EsperarYActualizarUI());
    }

    // Esto obliga a Unity a esperar a que la escena se asiente antes de buscar las imágenes
    System.Collections.IEnumerator EsperarYActualizarUI()
    {
        // Espera el final del frame actual para que todo en la escena ya exista
        yield return new WaitForEndOfFrame();

        RecolectorIngredientes recolectorActual = FindFirstObjectByType<RecolectorIngredientes>();
        if (recolectorActual != null)
        {
            recolectorActual.inventarioUI = this;
            recolectorActual.ActualizarUI(); // Ahora sí, dibuja con las imágenes listas
        }
    }
    public void ToggleMochila()
    {
        mochilaAbierta = !mochilaAbierta;
        panelMochila.SetActive(mochilaAbierta);
    }

    public void ToggleRecetas()
    {
        recetasAbiertas = !recetasAbiertas;
        seccionRecetas.SetActive(recetasAbiertas);
    }

    public void ActualizarUI(List<Sprite> iconos, List<int> cantidades)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < iconos.Count)
            {
                slots[i].sprite = iconos[i];
                slots[i].color = Color.white;

                if (i < textosCantidad.Count && textosCantidad[i] != null)
                {
                    if (cantidades[i] > 1)
                    {
                        textosCantidad[i].text = "x" + cantidades[i];
                        textosCantidad[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        textosCantidad[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                slots[i].sprite = null;
                slots[i].color = new Color(1, 1, 1, 0.2f);

                if (i < textosCantidad.Count && textosCantidad[i] != null)
                    textosCantidad[i].gameObject.SetActive(false);
            }
        }
    }

}