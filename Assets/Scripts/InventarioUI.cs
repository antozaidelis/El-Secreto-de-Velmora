using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventarioUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelMochila;
    public List<Image> slots;
    public List<TextMeshProUGUI> textosCantidad; // un texto de cantidad por cada slot, mismo orden

    private bool mochilaAbierta = false;

    public void ToggleMochila()
    {
        mochilaAbierta = !mochilaAbierta;
        panelMochila.SetActive(mochilaAbierta);
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
                    // Si hay más de 1, mostramos "x2", "x3", etc. Si hay solo 1, no mostramos nada.
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