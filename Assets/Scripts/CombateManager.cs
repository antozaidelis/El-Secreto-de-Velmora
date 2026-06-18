using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CombateManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelDialogo;
    public GameObject panelBatalla;

    [Header("Diálogo del enemigo")]
    public Image retratoEnemigoChico;
    public TextMeshProUGUI nombreEnemigoTexto;
    public TextMeshProUGUI textoDialogo;
    public Button botonSkip;
    public string nombreEnemigo = "Hurón del Pensamiento";
    [TextArea]
    public string[] frasesEnemigo = new string[]
    {
        "Las flores tiemblan... significa que el bosque ya te vio, pequeño Chef.",
        "No eres el primero que viene a probar suerte aquí.",
        "Veamos si tus recetas son tan buenas como dicen."
    };
    public Sprite[] expresionesPorFrase; // mismo orden y cantidad que frasesEnemigo
    private int indiceFraseActual = 0;

    [Header("Vida")]
    public int vidaJugadorMax = 100;
    public int vidaEnemigoMax = 50;
    private int vidaJugador;
    private int vidaEnemigo;

    [Header("UI Batalla - Vida")]
    public TextMeshProUGUI textoVidaJugador;
    public TextMeshProUGUI textoVidaEnemigo;

    [Header("UI Batalla - Cara de Misu")]
    public Image caraMisu;
    public Sprite caraMisuNormal;
    public Sprite caraMisuGanando;
    public Sprite caraMisuPerdiendo;

    [Header("UI Batalla - Cara del Enemigo")]
    public Image caraEnemigoBatalla;
    public Sprite caraEnemigoNormal;
    public Sprite caraEnemigoGanando;
    public Sprite caraEnemigoPerdiendo;

    [Header("UI Batalla - Botones de recetas")]
    public Button botonSalteadoPicante;
    public Button botonSopaReconfortante;
    public Button botonInfusionAmarga;

    [Header("Configuración de recetas")]
    public int danioSalteadoPicante = 15;
    public int danioSalteadoPicanteMax = 30;
    public int curacionSopaReconfortante = 0; // se calcula según carga, ver lógica abajo
    public int curacionSopaReconfortanteMax = 40;
    public int danioInfusionAmarga = 5;
    public int danioInfusionAmargaMax = 10;

    [Header("Sistema de cargas")]
    public float cargaMaxima = 5f;
    public string ingredienteSalteadoPicante = "pimiento_0";
    public string ingredienteSopaReconfortante = "hierba_serena";
    public string ingredienteInfusionAmarga = "hongo_amargo";

    private float cargaSalteadoPicante = 0f;
    private float cargaSopaReconfortante = 0f;
    private float cargaInfusionAmarga = 0f;

    // Cuánta carga otorga cada cantidad de ingredientes gastados (de a 1)
    // 1 ingrediente = 10%, 2 = 50%, 3 = 100% de UNA barrita
    private float[] tablaCargaPorCantidad = new float[] { 0f, 0.1f, 0.5f, 1.0f };

    [Header("Inventario (referencia)")]
    public RecolectorIngredientes recolector; // arrastrar el GameObject del jugador que tiene este script

    [Header("UI - Barras de carga (opcional por ahora)")]
    public TextMeshProUGUI textoCargaSalteado;
    public TextMeshProUGUI textoCargaSopa;
    public TextMeshProUGUI textoCargaInfusion;

    [Header("UI - Panel de selección de ingrediente")]
    public GameObject panelSeleccionIngrediente;
    public TextMeshProUGUI textoDescripcionReceta;
    public List<Button> botonesIngredientesInventario; // mismo orden que los slots del inventario
    public List<Image> iconosIngredientesInventario;
    public List<TextMeshProUGUI> cantidadesIngredientesInventario;

    // Descripciones cortas, dan la pista sin decir el nombre del ingrediente
    [TextArea] public string descripcionSalteadoPicante = "Un plato que arde con fuerza... necesita algo picante para tomar forma.";
    [TextArea] public string descripcionSopaReconfortante = "Un caldo suave y calmo... algo del bosque, tranquilo, lo haría perfecto.";
    [TextArea] public string descripcionInfusionAmarga = "Un brebaje de sabor extraño... algo terroso y amargo le daría su efecto.";

    private string recetaSiendoCargada = ""; // "salteado", "sopa" o "infusion"

    [Header("Configuración enemigo")]
    public int danioAtaqueEnemigo = 10;

    private bool esTurnoJugador = true;
    private bool combateTerminado = false;

    void Start()
    {
        // Arranca mostrando el diálogo, batalla oculta
        panelDialogo.SetActive(true);
        panelBatalla.SetActive(false);

        nombreEnemigoTexto.text = nombreEnemigo;
        indiceFraseActual = 0;
        MostrarFraseActual();

        botonSkip.onClick.AddListener(AvanzarDialogo);

        botonSalteadoPicante.onClick.AddListener(UsarSalteadoPicante);
        botonSopaReconfortante.onClick.AddListener(UsarSopaReconfortante);
        botonInfusionAmarga.onClick.AddListener(UsarInfusionAmarga);

        if (panelSeleccionIngrediente != null)
            panelSeleccionIngrediente.SetActive(false);

        ActualizarTextosCarga();
    }

    // ---------- DIÁLOGO ----------

    private void MostrarFraseActual()
    {
        textoDialogo.text = frasesEnemigo[indiceFraseActual];

        if (retratoEnemigoChico != null && expresionesPorFrase != null
            && indiceFraseActual < expresionesPorFrase.Length
            && expresionesPorFrase[indiceFraseActual] != null)
        {
            retratoEnemigoChico.sprite = expresionesPorFrase[indiceFraseActual];
        }
    }

    public void AvanzarDialogo()
    {
        indiceFraseActual++;

        if (indiceFraseActual < frasesEnemigo.Length)
        {
            MostrarFraseActual();
        }
        else
        {
            IniciarBatalla();
        }
    }

    // ---------- TRANSICIÓN DIÁLOGO → BATALLA ----------

    public void IniciarBatalla()
    {
        panelDialogo.SetActive(false);
        panelBatalla.SetActive(true);

        vidaJugador = vidaJugadorMax;
        vidaEnemigo = vidaEnemigoMax;
        combateTerminado = false;
        esTurnoJugador = true;

        ActualizarUIVida();
        ActualizarCaraMisu();
        ActualizarCaraEnemigo();
    }

    // ---------- RECETAS: USAR ----------

    public void UsarSalteadoPicante()
    {
        if (!PuedeJugar()) return;

        if (cargaSalteadoPicante <= 0f)
        {
            Debug.Log("Salteado Picante no tiene carga. Cargalo antes de usarlo.");
            return;
        }

        int danio = CalcularEfectoEscalado(danioSalteadoPicante, danioSalteadoPicanteMax, cargaSalteadoPicante);
        AplicarDanioAlEnemigo(danio, "Salteado Picante");

        cargaSalteadoPicante = 0f;
        ActualizarTextosCarga();
    }

    public void UsarSopaReconfortante()
    {
        if (!PuedeJugar()) return;

        if (cargaSopaReconfortante <= 0f)
        {
            Debug.Log("Sopa Reconfortante no tiene carga. Cargala antes de usarla.");
            return;
        }

        int curacion = CalcularEfectoEscalado(0, curacionSopaReconfortanteMax, cargaSopaReconfortante);

        vidaJugador += curacion;
        if (vidaJugador > vidaJugadorMax) vidaJugador = vidaJugadorMax;

        Debug.Log("Usaste Sopa Reconfortante. Te curaste " + curacion + " HP.");
        ActualizarUIVida();
        ActualizarCaraMisu();
        ActualizarCaraEnemigo();

        cargaSopaReconfortante = 0f;
        ActualizarTextosCarga();

        PasarTurnoEnemigo();
    }

    public void UsarInfusionAmarga()
    {
        if (!PuedeJugar()) return;

        if (cargaInfusionAmarga <= 0f)
        {
            Debug.Log("Infusión Amarga no tiene carga. Cargala antes de usarla.");
            return;
        }

        int danio = CalcularEfectoEscalado(danioInfusionAmarga, danioInfusionAmargaMax, cargaInfusionAmarga);
        AplicarDanioAlEnemigo(danio, "Infusión Amarga");

        cargaInfusionAmarga = 0f;
        ActualizarTextosCarga();
    }

    // Interpola linealmente entre el valor mínimo (carga ~0) y máximo (carga llena)
    private int CalcularEfectoEscalado(int valorMinimo, int valorMaximo, float cargaActual)
    {
        float porcentaje = Mathf.Clamp01(cargaActual / cargaMaxima);
        int resultado = Mathf.RoundToInt(Mathf.Lerp(valorMinimo, valorMaximo, porcentaje));
        return resultado;
    }

    // ---------- RECETAS: ABRIR PANEL DE CARGA ----------

    // Estos van conectados a los botones "+" de cada receta
    public void AbrirCargaSalteadoPicante()
    {
        if (!PuedeAbrirCarga()) return;
        recetaSiendoCargada = "salteado";
        textoDescripcionReceta.text = descripcionSalteadoPicante;
        MostrarPanelSeleccionIngrediente();
    }

    public void AbrirCargaSopaReconfortante()
    {
        if (!PuedeAbrirCarga()) return;
        recetaSiendoCargada = "sopa";
        textoDescripcionReceta.text = descripcionSopaReconfortante;
        MostrarPanelSeleccionIngrediente();
    }

    public void AbrirCargaInfusionAmarga()
    {
        if (!PuedeAbrirCarga()) return;
        recetaSiendoCargada = "infusion";
        textoDescripcionReceta.text = descripcionInfusionAmarga;
        MostrarPanelSeleccionIngrediente();
    }

    // Fuera de combate (esTurnoJugador y combateTerminado no aplican) siempre se puede cargar.
    // Dentro de combate, solo si es el turno del jugador y el combate no terminó.
    private bool PuedeAbrirCarga()
    {
        if (panelBatalla != null && panelBatalla.activeSelf)
            return PuedeJugar();

        return true; // fuera de combate, siempre se puede intentar cargar
    }

    private void MostrarPanelSeleccionIngrediente()
    {
        if (panelSeleccionIngrediente == null || recolector == null) return;

        panelSeleccionIngrediente.SetActive(true);

        // Recorremos los slots actuales del inventario y los mostramos en el panel de selección
        for (int i = 0; i < botonesIngredientesInventario.Count; i++)
        {
            if (i < recolector.slotsInventario.Count)
            {
                SlotInventario slot = recolector.slotsInventario[i];

                botonesIngredientesInventario[i].gameObject.SetActive(true);

                if (i < iconosIngredientesInventario.Count)
                    iconosIngredientesInventario[i].sprite = recolector.ObtenerIconoDe(slot.nombreIngrediente);

                if (i < cantidadesIngredientesInventario.Count)
                    cantidadesIngredientesInventario[i].text = "x" + slot.cantidad;

                // Guardamos qué ingrediente representa este botón, para usarlo al hacer click
                string nombreIngrediente = slot.nombreIngrediente;
                botonesIngredientesInventario[i].onClick.RemoveAllListeners();
                botonesIngredientesInventario[i].onClick.AddListener(() => ElegirIngredienteParaCarga(nombreIngrediente));
            }
            else
            {
                botonesIngredientesInventario[i].gameObject.SetActive(false);
            }
        }
    }

    private void ElegirIngredienteParaCarga(string nombreIngredienteElegido)
    {
        panelSeleccionIngrediente.SetActive(false);

        string ingredienteCorrecto = ObtenerIngredienteDeReceta(recetaSiendoCargada);
        bool esCorrecto = (nombreIngredienteElegido == ingredienteCorrecto);

        int cantidadDisponible = recolector.ContarIngrediente(nombreIngredienteElegido);
        int cantidadAGastar = Mathf.Min(3, cantidadDisponible);

        for (int i = 0; i < cantidadAGastar; i++)
            recolector.GastarIngrediente(nombreIngredienteElegido);

        if (esCorrecto)
        {
            float cargaGanada = tablaCargaPorCantidad[cantidadAGastar];
            SumarCarga(recetaSiendoCargada, cargaGanada);

            Debug.Log("¡Acertaste! Cargaste " + (cargaGanada * 100f) + "% de una barrita con " + cantidadAGastar + " ingrediente(s).");

            // Si estamos en combate: no recibe daño este turno, pero pasa el turno igual (sin ataque enemigo)
            if (panelBatalla != null && panelBatalla.activeSelf)
            {
                esTurnoJugador = false;
                esTurnoJugador = true; // no ataca el enemigo, vuelve a ser turno del jugador directamente
            }
        }
        else
        {
            Debug.Log("Elegiste mal. Perdiste " + cantidadAGastar + " de ese ingrediente.");

            // Si estamos en combate: pierde el turno Y el enemigo ataca
            if (panelBatalla != null && panelBatalla.activeSelf)
            {
                PasarTurnoEnemigo();
            }
        }

        recetaSiendoCargada = "";
    }

    private string ObtenerIngredienteDeReceta(string receta)
    {
        switch (receta)
        {
            case "salteado": return ingredienteSalteadoPicante;
            case "sopa": return ingredienteSopaReconfortante;
            case "infusion": return ingredienteInfusionAmarga;
            default: return "";
        }
    }

    private void SumarCarga(string receta, float cantidad)
    {
        switch (receta)
        {
            case "salteado":
                cargaSalteadoPicante = Mathf.Min(cargaSalteadoPicante + cantidad, cargaMaxima);
                break;
            case "sopa":
                cargaSopaReconfortante = Mathf.Min(cargaSopaReconfortante + cantidad, cargaMaxima);
                break;
            case "infusion":
                cargaInfusionAmarga = Mathf.Min(cargaInfusionAmarga + cantidad, cargaMaxima);
                break;
        }
        ActualizarTextosCarga();
    }

    private void ActualizarTextosCarga()
    {
        if (textoCargaSalteado != null)
            textoCargaSalteado.text = cargaSalteadoPicante.ToString("0.0") + "/" + cargaMaxima.ToString("0");
        if (textoCargaSopa != null)
            textoCargaSopa.text = cargaSopaReconfortante.ToString("0.0") + "/" + cargaMaxima.ToString("0");
        if (textoCargaInfusion != null)
            textoCargaInfusion.text = cargaInfusionAmarga.ToString("0.0") + "/" + cargaMaxima.ToString("0");
    }

    private bool PuedeJugar()
    {
        return esTurnoJugador && !combateTerminado;
    }

    public void CerrarPanelSeleccion()
    {
        if (panelSeleccionIngrediente != null)
            panelSeleccionIngrediente.SetActive(false);

        recetaSiendoCargada = "";
    }

    private void AplicarDanioAlEnemigo(int danio, string nombreReceta)
    {
        vidaEnemigo -= danio;
        if (vidaEnemigo < 0) vidaEnemigo = 0;

        Debug.Log("Usaste " + nombreReceta + ". Hiciste " + danio + " de daño.");
        ActualizarUIVida();
        ActualizarCaraEnemigo();

        if (vidaEnemigo <= 0)
        {
            TerminarCombate(true);
            return;
        }

        PasarTurnoEnemigo();
    }

    private void PasarTurnoEnemigo()
    {
        esTurnoJugador = false;
        Invoke("TurnoEnemigo", 1.2f);
    }

    private void TurnoEnemigo()
    {
        if (combateTerminado) return;

        vidaJugador -= danioAtaqueEnemigo;
        if (vidaJugador < 0) vidaJugador = 0;

        Debug.Log("El enemigo ataca. Te hizo " + danioAtaqueEnemigo + " de daño.");
        ActualizarUIVida();
        ActualizarCaraMisu();
        ActualizarCaraEnemigo();

        if (vidaJugador <= 0)
        {
            TerminarCombate(false);
            return;
        }

        esTurnoJugador = true;
    }

    // ---------- UI ----------

    private void ActualizarUIVida()
    {
        textoVidaJugador.text = "Misu: " + vidaJugador + " / " + vidaJugadorMax;
        textoVidaEnemigo.text = nombreEnemigo + ": " + vidaEnemigo + " / " + vidaEnemigoMax;
    }

    private void ActualizarCaraMisu()
    {
        if (caraMisu == null) return;

        float porcentajeVida = (float)vidaJugador / vidaJugadorMax;
        float porcentajeVidaEnemigo = (float)vidaEnemigo / vidaEnemigoMax;

        if (porcentajeVida <= 0.3f)
            caraMisu.sprite = caraMisuPerdiendo;
        else if (porcentajeVidaEnemigo < porcentajeVida)
            caraMisu.sprite = caraMisuGanando;
        else
            caraMisu.sprite = caraMisuNormal;
    }

    private void ActualizarCaraEnemigo()
    {
        if (caraEnemigoBatalla == null) return;

        float porcentajeVida = (float)vidaJugador / vidaJugadorMax;
        float porcentajeVidaEnemigo = (float)vidaEnemigo / vidaEnemigoMax;

        if (porcentajeVidaEnemigo <= 0.3f)
            caraEnemigoBatalla.sprite = caraEnemigoPerdiendo; // el enemigo está mal
        else if (porcentajeVida < porcentajeVidaEnemigo)
            caraEnemigoBatalla.sprite = caraEnemigoGanando; // el enemigo va ganando
        else
            caraEnemigoBatalla.sprite = caraEnemigoNormal;
    }

    private void TerminarCombate(bool ganoJugador)
    {
        combateTerminado = true;

        if (ganoJugador)
        {
            Debug.Log("¡Ganaste el combate!");
            if (caraMisu != null) caraMisu.sprite = caraMisuGanando;
            if (caraEnemigoBatalla != null) caraEnemigoBatalla.sprite = caraEnemigoPerdiendo;
        }
        else
        {
            Debug.Log("Has sido derrotado...");
            if (caraMisu != null) caraMisu.sprite = caraMisuPerdiendo;
            if (caraEnemigoBatalla != null) caraEnemigoBatalla.sprite = caraEnemigoGanando;
        }
    }
}