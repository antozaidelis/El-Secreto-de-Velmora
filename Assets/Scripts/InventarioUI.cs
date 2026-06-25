using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class InventarioUI : MonoBehaviour
{
    public static InventarioUI Instancia { get; private set; }

    [Header("Slots de la Hotbar")]
    public List<Image> slots;
    public List<TextMeshProUGUI> textosCantidad;

    [Header("Botones de cada slot (para elegir ingrediente en combate)")]
    public List<Button> botonesSlots;

    [Header("Feedback visual de selección (opcional)")]
    public Image fondoPanel;
    public Color colorNormal = new Color(1, 1, 1, 0f);
    public Color colorSeleccion = new Color(1, 0.9f, 0.3f, 0.3f);

    [Header("Tooltip de descripción")]
    public GameObject tooltipDescripcion;
    public TextMeshProUGUI textoTooltip;

    private bool suscrito = false;
    private System.Action<string> callbackSeleccion;

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

    void OnEnable()
    {
        SuscribirseAlGameManager();
    }

    void Start()
    {
        SuscribirseAlGameManager();
        ActualizarUI();
        ConfigurarHoverEnSlots();

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

        if (tooltipDescripcion != null)
            tooltipDescripcion.SetActive(false);
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

    public void Mostrar(bool mostrar)
    {
        gameObject.SetActive(mostrar);
    }

    public void HabilitarSeleccion(System.Action<string> callback)
    {
        callbackSeleccion = callback;

        if (fondoPanel != null) fondoPanel.color = colorSeleccion;

        for (int i = 0; i < botonesSlots.Count; i++)
        {
            int indice = i;
            botonesSlots[i].onClick.RemoveAllListeners();
            botonesSlots[i].onClick.AddListener(() => SeleccionarSlot(indice));
        }
    }

    public void DeshabilitarSeleccion()
    {
        callbackSeleccion = null;

        if (fondoPanel != null) fondoPanel.color = colorNormal;

        foreach (Button boton in botonesSlots)
        {
            if (boton != null)
                boton.onClick.RemoveAllListeners();
        }
    }

    private void SeleccionarSlot(int indice)
    {
        if (GameManager.Instancia == null || callbackSeleccion == null) return;

        List<SlotInventario> datos = GameManager.Instancia.slotsInventario;

        if (indice >= datos.Count) return;

        string nombreIngrediente = datos[indice].nombreIngrediente;
        callbackSeleccion.Invoke(nombreIngrediente);
    }

    // ---------- HOVER / TOOLTIP ----------

    private void ConfigurarHoverEnSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            int indice = i;
            GameObject slotObj = slots[i].gameObject;

            EventTrigger trigger = slotObj.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = slotObj.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            EventTrigger.Entry entradaEnter = new EventTrigger.Entry();
            entradaEnter.eventID = EventTriggerType.PointerEnter;
            entradaEnter.callback.AddListener((data) => { MostrarTooltip(indice); });
            trigger.triggers.Add(entradaEnter);

            EventTrigger.Entry entradaExit = new EventTrigger.Entry();
            entradaExit.eventID = EventTriggerType.PointerExit;
            entradaExit.callback.AddListener((data) => { OcultarTooltip(); });
            trigger.triggers.Add(entradaExit);
        }
    }

    private void MostrarTooltip(int indice)
    {
        if (GameManager.Instancia == null || tooltipDescripcion == null || textoTooltip == null) return;

        List<SlotInventario> datos = GameManager.Instancia.slotsInventario;

        if (indice >= datos.Count) return;

        string nombreLindo = GameManager.Instancia.ObtenerNombreParaMostrar(datos[indice].nombreIngrediente);
        string descripcion = GameManager.Instancia.ObtenerDescripcionDe(datos[indice].nombreIngrediente);

        textoTooltip.text = string.IsNullOrEmpty(descripcion) ? nombreLindo : nombreLindo + "\n" + descripcion;
        tooltipDescripcion.SetActive(true);
    }

    private void OcultarTooltip()
    {
        if (tooltipDescripcion != null)
            tooltipDescripcion.SetActive(false);
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
                slots[i].color = icono != null ? Color.white : new Color(1, 1, 1, 0f);

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
                slots[i].color = new Color(1, 1, 1, 0f);
                if (i < textosCantidad.Count && textosCantidad[i] != null)
                    textosCantidad[i].gameObject.SetActive(false);
            }
        }
    }
}