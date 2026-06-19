using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventarioUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelMochila;
    public List<Image> slots;

    private bool mochilaAbierta = false;

    public void ToggleMochila()
    {
        mochilaAbierta = !mochilaAbierta;
        panelMochila.SetActive(mochilaAbierta);
    }

    public void ActualizarUI(List<Sprite> iconos)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < iconos.Count)
            {
                slots[i].sprite = iconos[i];
                slots[i].color = Color.white;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].color = new Color(1, 1, 1, 0.2f);
            }
        }
    }
}
