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

        ActualizarUI();
    }

    void OnEnable()
    {
        if (GameManager.Instancia != null)
            GameManager.Instancia.OnInventarioCambiado += ActualizarUI;
    }

    void OnDisable()
    {
        if (GameManager.Instancia != null)
            GameManager.Instancia.OnInventarioCambiado -= ActualizarUI;
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

    public void ActualizarUI()
    {
        if (GameManager.Instancia == null) return;

        List<SlotInventario> datos = GameManager.Instancia.slotsInventario;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < datos.Count)
            {
                Sprite icono = GameManager.Instancia.ObtenerIconoDe(datos[i].nombreIngrediente);
                slots[i].sprite = icono;
                slots[i].color = icono != null ? Color.white : new Color(1, 1, 1, 0.2f);

                if (i < textosCantidad.Count && textosCantidad[i] != null)
                {
                    if (datos[i].cantidad > 1)
                    {
                        textosCantidad[i].text = "x" + datos[i].cantidad;
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