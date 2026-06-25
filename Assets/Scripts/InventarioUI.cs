using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventarioUI : MonoBehaviour
{
    public static InventarioUI Instancia { get; private set; }

    [Header("Slots de la Hotbar")]
    public List<Image> slots;
    public List<TextMeshProUGUI> textosCantidad;

    private bool suscrito = false;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SuscribirseAlGameManager();
        ActualizarUI();

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    void OnDisable()
    {
        if (GameManager.Instancia != null && suscrito)
        {
            GameManager.Instancia.OnInventarioCambiado -= ActualizarUI;
            suscrito = false;
        }
    }

    private void SuscribirseAlGameManager()
    {
        if (GameManager.Instancia != null && !suscrito)
        {
            GameManager.Instancia.OnInventarioCambiado += ActualizarUI;
            suscrito = true;
        }
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