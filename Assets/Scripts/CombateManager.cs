using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
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

    [Header("Frases durante la batalla (globo de diálogo)")]
    public GameObject globoDialogoBatalla;
    public TextMeshProUGUI textoGloboDialogoBatalla;
    [TextArea]
    public string[] frasesBatalla = new string[]
    {
        "El pensamiento no se vence con fuerza.",
        "Ya vi esa receta antes, más de una vez.",
        "¿Sabés por qué luchás, o solo seguís el instinto?",
        "El bosque enseña paciencia. Vos todavía no.",
        "No es la sartén la que decide, sino quien la sostiene.",
        "Saber cuándo retirarse también es una forma de sabiduría."
    };
    public int turnosMinimoEntreFrases = 2;
    public int turnosMaximoEntreFrases = 3;
    private int turnosHastaProximaFrase;

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

    [Header("UI - Barras de carga (texto, opcional)")]
    public TextMeshProUGUI textoCargaSalteado;
    public TextMeshProUGUI textoCargaSopa;
    public TextMeshProUGUI textoCargaInfusion;

    [Header("UI - Barras de carga (visual, FondoSalteadoPicante en mochila)")]
    public Image barraLlenaSalteado;
    public Image barraLlenaSopa;
    public Image barraLlenaInfusion;

    [Header("UI - Barras de carga (visual, copia en RecetasBatalla)")]
    public Image barraLlenaSalteadoBatalla;
    public Image barraLlenaSopaBatalla;
    public Image barraLlenaInfusionBatalla;

    [Header("UI - Tarjetas encendido/apagado (SeccionRecetas)")]
    public Image fondoTarjetaSalteado;
    public Image fondoTarjetaSopa;
    public Image fondoTarjetaInfusion;
    public Sprite spriteApagadoSalteado;
    public Sprite spriteEncendidoSalteado;
    public Sprite spriteApagadoSopa;
    public Sprite spriteEncendidoSopa;
    public Sprite spriteApagadoInfusion;
    public Sprite spriteEncendidoInfusion;

    [Header("UI - Tarjetas encendido/apagado (RecetasBatalla)")]
    public Image fondoTarjetaSalteadoBatalla;
    public Image fondoTarjetaSopaBatalla;
    public Image fondoTarjetaInfusionBatalla;

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

    [Header("Configuración enemigo - Hurón del Pensamiento")]
    public int danioMordida = 10;
    [Range(0f, 1f)] public float reduccionPosturaDefensiva = 0.25f; // 25% menos daño recibido mientras dura
    public int turnosDebilitamiento = 1;
    [Range(0f, 1f)] public float bonusDanioDebilitado = 0.25f; // 25% más daño recibido mientras está debilitado

    private bool enPosturaDefensiva = false;
    private int turnosDebilitamientoRestantes = 0;

    [Header("UI - Estado del enemigo (opcional)")]
    public TextMeshProUGUI textoEstadoEnemigo;

    [Header("Efecto de impacto (screen shake)")]
    public Transform camaraCombate; // arrastrar la Main Camera de la escena de combate
    public float duracionTemblor = 0.3f;
    public float intensidadTemblor = 0.15f;

    [Header("Panel de resultado")]
    public GameObject panelResultado;
    public TextMeshProUGUI textoResultado;
    public TextMeshProUGUI textoRecompensas;
    public Image iconoRecompensa1;
    public Image iconoRecompensa2;
    public Image iconoRecompensa3;
    public Button botonContinuar;
    public string nombreEscenaMapa = "Escena_Lara";
    [TextArea] public string fraseVictoria = "Las flores dejan de temblar. El Hurón se retira, dejando su ofrenda entre el pasto.";
    [TextArea] public string fraseDerrota = "El bosque te vio caer. Cuando despertás, tu mochila está vacía.";

    [Header("Recompensas al ganar")]
    public string ingredienteRecompensa1 = "flor del pensamiento";
    public int cantidadRecompensa1 = 1;
    public string ingredienteRecompensa2 = "pluma de calendula";
    public int cantidadRecompensa2 = 1;
    public string ingredienteRecompensa3 = "pimiento_0";
    public int cantidadRecompensa3 = 3;

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

        if (panelSeleccionIngrediente != null)
            panelSeleccionIngrediente.SetActive(false);

        if (panelResultado != null)
            panelResultado.SetActive(false);

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

    private void MostrarFraseDeBatalla()
    {
        if (frasesBatalla == null || frasesBatalla.Length == 0) return;
        if (globoDialogoBatalla == null || textoGloboDialogoBatalla == null) return;

        string frase = frasesBatalla[Random.Range(0, frasesBatalla.Length)];

        textoGloboDialogoBatalla.text = frase;
        globoDialogoBatalla.SetActive(true);
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

        turnosHastaProximaFrase = Random.Range(turnosMinimoEntreFrases, turnosMaximoEntreFrases + 1);

        if (globoDialogoBatalla != null)
            globoDialogoBatalla.SetActive(false);

        ActualizarUIVida();
        ActualizarCaraMisu();
        ActualizarCaraEnemigo();
    }

    // ---------- RECETAS: USAR ----------

    public void UsarSalteadoPicante()
    {
        if (!PuedeJugar()) return;

        if (globoDialogoBatalla != null) globoDialogoBatalla.SetActive(false);

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

        if (globoDialogoBatalla != null) globoDialogoBatalla.SetActive(false);

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

        if (globoDialogoBatalla != null) globoDialogoBatalla.SetActive(false);

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
                Debug.Log("SumarCarga SOPA: cantidad=" + cantidad + " | cargaSopaReconfortante ahora=" + cargaSopaReconfortante);
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

        // Actualiza las barras visuales (Fill Amount de 0 a 1)
        float fillSalteado = cargaSalteadoPicante / cargaMaxima;
        float fillSopa = cargaSopaReconfortante / cargaMaxima;
        float fillInfusion = cargaInfusionAmarga / cargaMaxima;

        if (barraLlenaSalteado != null) barraLlenaSalteado.fillAmount = fillSalteado;
        if (barraLlenaSopa != null) barraLlenaSopa.fillAmount = fillSopa;
        if (barraLlenaInfusion != null) barraLlenaInfusion.fillAmount = fillInfusion;

        // Actualiza también la copia de batalla, si está asignada
        if (barraLlenaSalteadoBatalla != null) barraLlenaSalteadoBatalla.fillAmount = fillSalteado;
        if (barraLlenaSopaBatalla != null) barraLlenaSopaBatalla.fillAmount = fillSopa;
        if (barraLlenaInfusionBatalla != null) barraLlenaInfusionBatalla.fillAmount = fillInfusion;

        // Cambia el sprite de fondo entre "apagado" y "encendido" según si llegó a 0.5 de carga absoluta o más
        float umbralEncendido = 0.5f;
        ActualizarEncendidoTarjeta(fondoTarjetaSalteado, cargaSalteadoPicante, umbralEncendido, spriteApagadoSalteado, spriteEncendidoSalteado);
        ActualizarEncendidoTarjeta(fondoTarjetaSopa, cargaSopaReconfortante, umbralEncendido, spriteApagadoSopa, spriteEncendidoSopa);
        ActualizarEncendidoTarjeta(fondoTarjetaInfusion, cargaInfusionAmarga, umbralEncendido, spriteApagadoInfusion, spriteEncendidoInfusion);

        ActualizarEncendidoTarjeta(fondoTarjetaSalteadoBatalla, cargaSalteadoPicante, umbralEncendido, spriteApagadoSalteado, spriteEncendidoSalteado);
        ActualizarEncendidoTarjeta(fondoTarjetaSopaBatalla, cargaSopaReconfortante, umbralEncendido, spriteApagadoSopa, spriteEncendidoSopa);
        ActualizarEncendidoTarjeta(fondoTarjetaInfusionBatalla, cargaInfusionAmarga, umbralEncendido, spriteApagadoInfusion, spriteEncendidoInfusion);
    }

    private void ActualizarEncendidoTarjeta(Image fondo, float cargaActual, float umbral, Sprite apagado, Sprite encendido)
    {
        if (fondo == null) return;

        if (cargaActual >= umbral && encendido != null)
            fondo.sprite = encendido;
        else if (apagado != null)
            fondo.sprite = apagado;
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
        float danioFinal = danio;

        // Si el Hurón está en Postura Defensiva, recibe menos daño este turno
        if (enPosturaDefensiva)
        {
            danioFinal *= (1f - reduccionPosturaDefensiva);
            Debug.Log("El Hurón está en Postura Defensiva, recibe menos daño.");
        }

        // Si está debilitado (por Infusión Amarga), recibe más daño de cualquier receta
        if (turnosDebilitamientoRestantes > 0)
        {
            danioFinal *= (1f + bonusDanioDebilitado);
            Debug.Log("El Hurón está debilitado, recibe más daño.");
        }

        int danioRedondeado = Mathf.RoundToInt(danioFinal);

        vidaEnemigo -= danioRedondeado;
        if (vidaEnemigo < 0) vidaEnemigo = 0;

        Debug.Log("Usaste " + nombreReceta + ". Hiciste " + danioRedondeado + " de daño.");

        if (camaraCombate != null)
            StartCoroutine(TemblorDeCamara());

        // Si la receta usada es Infusión Amarga, aplica el debilitamiento
        if (nombreReceta == "Infusión Amarga")
        {
            turnosDebilitamientoRestantes = turnosDebilitamiento;
            Debug.Log("El Hurón queda debilitado por " + turnosDebilitamiento + " turno(s).");
        }

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

        // Antes de actuar, la postura defensiva del turno anterior ya no aplica
        enPosturaDefensiva = false;

        // Chequea si corresponde mostrar una frase de batalla este turno
        turnosHastaProximaFrase--;
        if (turnosHastaProximaFrase <= 0)
        {
            MostrarFraseDeBatalla();
            turnosHastaProximaFrase = Random.Range(turnosMinimoEntreFrases, turnosMaximoEntreFrases + 1);
        }

        // Elige aleatoriamente entre Mordida (0) y Postura Defensiva (1)
        int accionElegida = Random.Range(0, 2);

        if (accionElegida == 0)
        {
            // Mordida
            vidaJugador -= danioMordida;
            if (vidaJugador < 0) vidaJugador = 0;

            Debug.Log("El Hurón usa Mordida. Te hizo " + danioMordida + " de daño.");
            if (textoEstadoEnemigo != null) textoEstadoEnemigo.text = "¡Mordida!";

            if (camaraCombate != null)
                StartCoroutine(TemblorDeCamara());
        }
        else
        {
            // Postura Defensiva: no ataca este turno, pero se prepara para recibir menos daño
            enPosturaDefensiva = true;

            Debug.Log("El Hurón adopta una Postura Defensiva.");
            if (textoEstadoEnemigo != null) textoEstadoEnemigo.text = "Postura Defensiva";
        }

        // Reduce los turnos de debilitamiento restantes (si los hay)
        if (turnosDebilitamientoRestantes > 0)
            turnosDebilitamientoRestantes--;

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

            // Recompensa: agrega los ingredientes ganados al inventario
            if (recolector != null)
            {
                for (int i = 0; i < cantidadRecompensa1; i++)
                    recolector.AgregarIngrediente(ingredienteRecompensa1);

                for (int i = 0; i < cantidadRecompensa2; i++)
                    recolector.AgregarIngrediente(ingredienteRecompensa2);

                for (int i = 0; i < cantidadRecompensa3; i++)
                    recolector.AgregarIngrediente(ingredienteRecompensa3);

                Debug.Log("Recompensas obtenidas: " + cantidadRecompensa1 + " " + ingredienteRecompensa1 +
                          ", " + cantidadRecompensa2 + " " + ingredienteRecompensa2 +
                          ", " + cantidadRecompensa3 + " " + ingredienteRecompensa3);
            }

            MostrarPanelResultado(fraseVictoria, ObtenerTextoRecompensas(), true);
        }
        else
        {
            Debug.Log("Has sido derrotado...");
            if (caraMisu != null) caraMisu.sprite = caraMisuPerdiendo;
            if (caraEnemigoBatalla != null) caraEnemigoBatalla.sprite = caraEnemigoGanando;

            // Penalización: vacía todo el inventario
            if (recolector != null)
                recolector.slotsInventario.Clear();

            MostrarPanelResultado(fraseDerrota, "Perdiste todos tus ingredientes.");
        }
    }

    private string ObtenerTextoRecompensas()
    {
        return "Obtuviste: " + cantidadRecompensa1 + " " + ingredienteRecompensa1 +
               ", " + cantidadRecompensa2 + " " + ingredienteRecompensa2 +
               ", " + cantidadRecompensa3 + " " + ingredienteRecompensa3;
    }

    private void MostrarPanelResultado(string frase, string recompensas = "", bool mostrarIconos = false)
    {
        if (panelBatalla != null) panelBatalla.SetActive(false);

        if (panelResultado != null)
        {
            panelResultado.SetActive(true);

            if (textoResultado != null)
                textoResultado.text = frase;

            if (textoRecompensas != null)
                textoRecompensas.text = recompensas;

            if (mostrarIconos && recolector != null)
            {
                if (iconoRecompensa1 != null)
                {
                    iconoRecompensa1.sprite = recolector.ObtenerIconoDe(ingredienteRecompensa1);
                    iconoRecompensa1.gameObject.SetActive(iconoRecompensa1.sprite != null);
                }
                if (iconoRecompensa2 != null)
                {
                    iconoRecompensa2.sprite = recolector.ObtenerIconoDe(ingredienteRecompensa2);
                    iconoRecompensa2.gameObject.SetActive(iconoRecompensa2.sprite != null);
                }
                if (iconoRecompensa3 != null)
                {
                    iconoRecompensa3.sprite = recolector.ObtenerIconoDe(ingredienteRecompensa3);
                    iconoRecompensa3.gameObject.SetActive(iconoRecompensa3.sprite != null);
                }
            }
            else
            {
                if (iconoRecompensa1 != null) iconoRecompensa1.gameObject.SetActive(false);
                if (iconoRecompensa2 != null) iconoRecompensa2.gameObject.SetActive(false);
                if (iconoRecompensa3 != null) iconoRecompensa3.gameObject.SetActive(false);
            }

            if (botonContinuar != null)
            {
                botonContinuar.onClick.RemoveAllListeners();
                botonContinuar.onClick.AddListener(VolverAlMapa);
            }
        }
    }

    private void VolverAlMapa()
    {
        SceneManager.LoadScene(nombreEscenaMapa);
    }

    private IEnumerator TemblorDeCamara()
    {
        Vector3 posicionOriginal = camaraCombate.localPosition;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionTemblor)
        {
            float offsetX = Random.Range(-1f, 1f) * intensidadTemblor;
            float offsetY = Random.Range(-1f, 1f) * intensidadTemblor;

            camaraCombate.localPosition = posicionOriginal + new Vector3(offsetX, offsetY, 0f);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        camaraCombate.localPosition = posicionOriginal;
    }
}