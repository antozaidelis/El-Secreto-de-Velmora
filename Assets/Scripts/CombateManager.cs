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
    public Sprite[] expresionesPorFrase;
    private int indiceFraseActual = 0;

    [Header("Efecto máquina de escribir")]
    public float velocidadEscritura = 0.03f;
    private Coroutine corrutinaEscritura;

    [Header("Sonido de máquina de escribir")]
    public AudioSource audioMaquinaEscribir;
    public AudioClip clipMaquinaEscribir;

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

    [Header("UI Batalla - Barras de vida")]
    public Image barraVidaJugador;
    public Image barraVidaEnemigo;

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

    [Header("Sonidos de estado del enemigo")]
    public AudioSource audioEstadoEnemigo;
    public AudioClip clipHuronGanando;
    public AudioClip clipHuronPerdiendo;
    private string estadoAnteriorEnemigo = "";

    [Header("UI Batalla - Botones de recetas")]
    public Button botonSalteadoPicante;
    public Button botonSopaReconfortante;
    public Button botonInfusionAmarga;

    [Header("Ataque Básico")]
    public int danioAtaqueBasico = 5;

    [Header("Configuración de recetas")]
    public int danioSalteadoPicante = 15;
    public int danioSalteadoPicanteMax = 28;
    public int curacionSopaReconfortante = 0;
    public int curacionSopaReconfortanteMax = 40;
    public int danioInfusionAmarga = 5;
    public int danioInfusionAmargaMax = 14;

    [Header("Sistema de cargas")]
    public float cargaMaxima = 5f;
    public string ingredienteSalteadoPicante = "pimiento_0";
    public string ingredienteSopaReconfortante = "hierba Serena";
    public string ingredienteInfusionAmarga = "hongo amargo";

    private float cargaSalteadoPicante = 0f;
    private float cargaSopaReconfortante = 0f;
    private float cargaInfusionAmarga = 0f;

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

    [Header("UI - Descripción de receta (opcional)")]
    public TextMeshProUGUI textoDescripcionReceta;

    [TextArea] public string descripcionSalteadoPicante = "Un plato que arde con fuerza... necesita algo picante para tomar forma.";
    [TextArea] public string descripcionSopaReconfortante = "Un caldo suave y calmo... algo del bosque, tranquilo, lo haría perfecto.";
    [TextArea] public string descripcionInfusionAmarga = "Un brebaje de sabor extraño... algo terroso y amargo le daría su efecto.";

    private string recetaSiendoCargada = "";

    [Header("Panel de selección de ingrediente (dinámico)")]
    public GameObject panelSeleccion;
    public GameObject botonIngredientePrefab;
    private List<GameObject> botonesInstanciados = new List<GameObject>();

    [Header("Configuración enemigo - Hurón del Pensamiento")]
    public int danioMordida = 10;
    [Range(0f, 1f)] public float reduccionPosturaDefensiva = 0.25f;
    public int turnosDebilitamiento = 1;
    [Range(0f, 1f)] public float bonusDanioDebilitado = 0.25f;

    private bool enPosturaDefensiva = false;
    private int turnosDebilitamientoRestantes = 0;

    [Header("UI - Estado del enemigo (opcional)")]
    public TextMeshProUGUI textoEstadoEnemigo;

    [Header("Efecto de impacto (screen shake)")]
    public Transform camaraCombate;
    public float duracionTemblor = 0.3f;
    public float intensidadTemblor = 0.15f;

    [Header("Panel de resultado")]
    public GameObject panelResultado;
    public TextMeshProUGUI textoResultado;
    public TextMeshProUGUI textoRecompensas;
    public Image iconoRecompensa1;
    public Image iconoRecompensa2;
    public Image iconoRecompensa3;
    public GameObject filaIconosRecompensa;
    public GameObject textoSinRecompensa;
    public Button botonContinuar;
    public string nombreEscenaMapa = "Escena_Lara";
    [TextArea] public string fraseVictoria = "Las flores dejan de temblar. El Hurón se retira, dejando su ofrenda entre el pasto.";
    [TextArea] public string fraseDerrota = "El bosque te vio caer. Cuando despertás, tu mochila está vacía.";

    [Header("Sprites del botón continuar (siguiente/reintentar)")]
    public Image imagenBotonContinuar;
    public Sprite spriteBotonSiguiente;
    public Sprite spriteBotonReintentar;

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
        panelDialogo.SetActive(true);
        panelBatalla.SetActive(false);

        if (InventarioUI.Instancia != null)
            InventarioUI.Instancia.Mostrar(false);

        nombreEnemigoTexto.text = nombreEnemigo;
        indiceFraseActual = 0;
        MostrarFraseActual();

        botonSkip.onClick.AddListener(AvanzarDialogo);

        if (panelResultado != null)
            panelResultado.SetActive(false);

        if (panelSeleccion != null)
            panelSeleccion.SetActive(false);

        if (textoDescripcionReceta != null)
            textoDescripcionReceta.gameObject.SetActive(false);

        ActualizarTextosCarga();
    }

    private void MostrarFraseActual()
    {
        if (corrutinaEscritura != null)
            StopCoroutine(corrutinaEscritura);

        corrutinaEscritura = StartCoroutine(EscribirTexto(frasesEnemigo[indiceFraseActual]));

        if (retratoEnemigoChico != null && expresionesPorFrase != null
            && indiceFraseActual < expresionesPorFrase.Length
            && expresionesPorFrase[indiceFraseActual] != null)
        {
            retratoEnemigoChico.sprite = expresionesPorFrase[indiceFraseActual];
        }
    }

    private IEnumerator EscribirTexto(string texto)
    {
        textoDialogo.text = "";
        string[] palabras = texto.Split(' ');
        bool primera = true;

        foreach (string palabra in palabras)
        {
            if (!primera)
                textoDialogo.text += " ";
            primera = false;

            if (audioMaquinaEscribir != null && clipMaquinaEscribir != null)
            {
                audioMaquinaEscribir.pitch = Random.Range(0.95f, 1.05f);
                audioMaquinaEscribir.clip = clipMaquinaEscribir;
                audioMaquinaEscribir.Play();
            }

            foreach (char letra in palabra)
            {
                textoDialogo.text += letra;
                yield return new WaitForSeconds(velocidadEscritura);
            }
        }

        corrutinaEscritura = null;
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
        if (corrutinaEscritura != null)
        {
            StopCoroutine(corrutinaEscritura);
            corrutinaEscritura = null;
            textoDialogo.text = frasesEnemigo[indiceFraseActual];

            if (audioMaquinaEscribir != null && audioMaquinaEscribir.isPlaying)
                audioMaquinaEscribir.Stop();

            return;
        }

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

    public void IniciarBatalla()
    {
        if (audioMaquinaEscribir != null && audioMaquinaEscribir.isPlaying)
            audioMaquinaEscribir.Stop();

        panelDialogo.SetActive(false);
        panelBatalla.SetActive(true);

        if (InventarioUI.Instancia != null)
            InventarioUI.Instancia.Mostrar(true);

        vidaJugador = vidaJugadorMax;
        vidaEnemigo = vidaEnemigoMax;
        combateTerminado = false;
        esTurnoJugador = true;
        estadoAnteriorEnemigo = "";

        turnosHastaProximaFrase = Random.Range(turnosMinimoEntreFrases, turnosMaximoEntreFrases + 1);

        if (globoDialogoBatalla != null)
            globoDialogoBatalla.SetActive(false);

        ActualizarUIVida();
        ActualizarCaraMisu();
        ActualizarCaraEnemigo();
    }

    public void UsarAtaqueBasico()
    {
        if (!PuedeJugar()) return;
        if (globoDialogoBatalla != null) globoDialogoBatalla.SetActive(false);

        AplicarDanioAlEnemigo(danioAtaqueBasico, "Ataque Básico");
    }

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

    private int CalcularEfectoEscalado(int valorMinimo, int valorMaximo, float cargaActual)
    {
        float porcentaje = Mathf.Clamp01(cargaActual / cargaMaxima);
        int resultado = Mathf.RoundToInt(Mathf.Lerp(valorMinimo, valorMaximo, porcentaje));
        return resultado;
    }

    // ---------- BOTÓN "+": abre/cierra el panel de selección de ingrediente ----------

    public void AbrirCargaSalteadoPicante()
    {
        if (!PuedeAbrirCarga()) return;

        if (recetaSiendoCargada == "salteado")
        {
            CancelarSeleccion();
            return;
        }

        IniciarSeleccion("salteado", descripcionSalteadoPicante);
    }

    public void AbrirCargaSopaReconfortante()
    {
        if (!PuedeAbrirCarga()) return;

        if (recetaSiendoCargada == "sopa")
        {
            CancelarSeleccion();
            return;
        }

        IniciarSeleccion("sopa", descripcionSopaReconfortante);
    }

    public void AbrirCargaInfusionAmarga()
    {
        if (!PuedeAbrirCarga()) return;

        if (recetaSiendoCargada == "infusion")
        {
            CancelarSeleccion();
            return;
        }

        IniciarSeleccion("infusion", descripcionInfusionAmarga);
    }

    private void IniciarSeleccion(string receta, string descripcion)
    {
        recetaSiendoCargada = receta;

        if (textoDescripcionReceta != null)
        {
            textoDescripcionReceta.text = descripcion;
            textoDescripcionReceta.gameObject.SetActive(true);
        }

        PoblarPanelSeleccion();

        if (panelSeleccion != null)
            panelSeleccion.SetActive(true);
    }

    private void CancelarSeleccion()
    {
        recetaSiendoCargada = "";

        if (textoDescripcionReceta != null)
            textoDescripcionReceta.gameObject.SetActive(false);

        if (panelSeleccion != null)
            panelSeleccion.SetActive(false);
    }

    private void PoblarPanelSeleccion()
    {
        foreach (GameObject boton in botonesInstanciados)
            Destroy(boton);
        botonesInstanciados.Clear();

        if (GameManager.Instancia == null || panelSeleccion == null || botonIngredientePrefab == null) return;

        List<SlotInventario> slots = GameManager.Instancia.slotsInventario;

        foreach (SlotInventario slot in slots)
        {
            GameObject nuevoBoton = Instantiate(botonIngredientePrefab, panelSeleccion.transform);
            nuevoBoton.SetActive(true);

            Image[] imagenes = nuevoBoton.GetComponentsInChildren<Image>();
            foreach (Image img in imagenes)
            {
                if (img.gameObject != nuevoBoton)
                {
                    img.sprite = GameManager.Instancia.ObtenerIconoDe(slot.nombreIngrediente);
                    img.color = img.sprite != null ? Color.white : new Color(1, 1, 1, 0f);
                }
            }

            TextMeshProUGUI texto = nuevoBoton.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null)
            {
                texto.text = slot.cantidad > 1 ? "x" + slot.cantidad : "";
            }

            string nombreIngrediente = slot.nombreIngrediente;
            Button boton = nuevoBoton.GetComponent<Button>();
            if (boton != null)
            {
                boton.onClick.RemoveAllListeners();
                boton.onClick.AddListener(() => ElegirIngredienteParaCarga(nombreIngrediente));
            }

            botonesInstanciados.Add(nuevoBoton);
        }
    }

    private bool PuedeAbrirCarga()
    {
        if (panelBatalla != null && panelBatalla.activeSelf)
            return PuedeJugar();

        return true;
    }

    private void ElegirIngredienteParaCarga(string nombreIngredienteElegido)
    {
        string ingredienteCorrecto = ObtenerIngredienteDeReceta(recetaSiendoCargada);
        bool esCorrecto = (nombreIngredienteElegido == ingredienteCorrecto);

        GameManager.Instancia.GastarIngrediente(nombreIngredienteElegido);

        if (esCorrecto)
        {
            SumarCarga(recetaSiendoCargada, cargaMaxima);

            if (panelBatalla != null && panelBatalla.activeSelf)
            {
                esTurnoJugador = true;
            }
        }
        else
        {
            if (panelBatalla != null && panelBatalla.activeSelf)
            {
                PasarTurnoEnemigo();
            }
        }

        CancelarSeleccion();
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

        float fillSalteado = cargaSalteadoPicante / cargaMaxima;
        float fillSopa = cargaSopaReconfortante / cargaMaxima;
        float fillInfusion = cargaInfusionAmarga / cargaMaxima;

        if (barraLlenaSalteado != null) barraLlenaSalteado.fillAmount = fillSalteado;
        if (barraLlenaSopa != null) barraLlenaSopa.fillAmount = fillSopa;
        if (barraLlenaInfusion != null) barraLlenaInfusion.fillAmount = fillInfusion;

        if (barraLlenaSalteadoBatalla != null) barraLlenaSalteadoBatalla.fillAmount = fillSalteado;
        if (barraLlenaSopaBatalla != null) barraLlenaSopaBatalla.fillAmount = fillSopa;
        if (barraLlenaInfusionBatalla != null) barraLlenaInfusionBatalla.fillAmount = fillInfusion;

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

    private void AplicarDanioAlEnemigo(int danio, string nombreReceta)
    {
        float danioFinal = danio;

        if (enPosturaDefensiva)
        {
            danioFinal *= (1f - reduccionPosturaDefensiva);
        }

        if (turnosDebilitamientoRestantes > 0)
        {
            danioFinal *= (1f + bonusDanioDebilitado);
        }

        int danioRedondeado = Mathf.RoundToInt(danioFinal);

        vidaEnemigo -= danioRedondeado;
        if (vidaEnemigo < 0) vidaEnemigo = 0;

        if (camaraCombate != null)
            StartCoroutine(TemblorDeCamara());

        if (nombreReceta == "Infusión Amarga")
        {
            turnosDebilitamientoRestantes = turnosDebilitamiento;
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

        enPosturaDefensiva = false;

        turnosHastaProximaFrase--;
        if (turnosHastaProximaFrase <= 0)
        {
            MostrarFraseDeBatalla();
            turnosHastaProximaFrase = Random.Range(turnosMinimoEntreFrases, turnosMaximoEntreFrases + 1);
        }

        int accionElegida = Random.Range(0, 2);

        if (accionElegida == 0)
        {
            vidaJugador -= danioMordida;
            if (vidaJugador < 0) vidaJugador = 0;

            if (textoEstadoEnemigo != null) textoEstadoEnemigo.text = "¡Mordida!";

            if (camaraCombate != null)
                StartCoroutine(TemblorDeCamara());
        }
        else
        {
            enPosturaDefensiva = true;
            if (textoEstadoEnemigo != null) textoEstadoEnemigo.text = "Postura Defensiva";
        }

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

    private void ActualizarUIVida()
    {
        textoVidaJugador.text = vidaJugador + " / " + vidaJugadorMax;
        textoVidaEnemigo.text = vidaEnemigo + " / " + vidaEnemigoMax;

        if (barraVidaJugador != null)
            barraVidaJugador.fillAmount = (float)vidaJugador / vidaJugadorMax;

        if (barraVidaEnemigo != null)
            barraVidaEnemigo.fillAmount = (float)vidaEnemigo / vidaEnemigoMax;
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

        string estadoActual;

        if (porcentajeVidaEnemigo <= 0.3f)
        {
            caraEnemigoBatalla.sprite = caraEnemigoPerdiendo;
            estadoActual = "perdiendo";
        }
        else if (porcentajeVida < porcentajeVidaEnemigo)
        {
            caraEnemigoBatalla.sprite = caraEnemigoGanando;
            estadoActual = "ganando";
        }
        else
        {
            caraEnemigoBatalla.sprite = caraEnemigoNormal;
            estadoActual = "normal";
        }

        if (estadoActual != estadoAnteriorEnemigo)
        {
            if (estadoActual == "ganando" && audioEstadoEnemigo != null && clipHuronGanando != null)
                audioEstadoEnemigo.PlayOneShot(clipHuronGanando);
            else if (estadoActual == "perdiendo" && audioEstadoEnemigo != null && clipHuronPerdiendo != null)
                audioEstadoEnemigo.PlayOneShot(clipHuronPerdiendo);

            estadoAnteriorEnemigo = estadoActual;
        }
    }

    private void TerminarCombate(bool ganoJugador)
    {
        combateTerminado = true;

        if (ganoJugador)
        {
            if (GameManager.Instancia != null)
                GameManager.Instancia.huronDerrotado = true;

            if (caraMisu != null) caraMisu.sprite = caraMisuGanando;
            if (caraEnemigoBatalla != null) caraEnemigoBatalla.sprite = caraEnemigoPerdiendo;

            if (GameManager.Instancia != null)
            {
                for (int i = 0; i < cantidadRecompensa1; i++)
                    GameManager.Instancia.AgregarIngrediente(ingredienteRecompensa1);

                for (int i = 0; i < cantidadRecompensa2; i++)
                    GameManager.Instancia.AgregarIngrediente(ingredienteRecompensa2);

                for (int i = 0; i < cantidadRecompensa3; i++)
                    GameManager.Instancia.AgregarIngrediente(ingredienteRecompensa3);
            }

            MostrarPanelResultado(fraseVictoria, ObtenerTextoRecompensas(), true);
        }
        else
        {
            if (caraMisu != null) caraMisu.sprite = caraMisuPerdiendo;
            if (caraEnemigoBatalla != null) caraEnemigoBatalla.sprite = caraEnemigoGanando;

            if (GameManager.Instancia != null)
                GameManager.Instancia.VaciarInventario();

            MostrarPanelResultado(fraseDerrota, "Perdiste todos tus ingredientes.");
        }
    }

    private string ObtenerTextoRecompensas()
    {
        string nombre1 = GameManager.Instancia != null ? GameManager.Instancia.ObtenerNombreParaMostrar(ingredienteRecompensa1) : ingredienteRecompensa1;
        string nombre2 = GameManager.Instancia != null ? GameManager.Instancia.ObtenerNombreParaMostrar(ingredienteRecompensa2) : ingredienteRecompensa2;
        string nombre3 = GameManager.Instancia != null ? GameManager.Instancia.ObtenerNombreParaMostrar(ingredienteRecompensa3) : ingredienteRecompensa3;

        return "Obtuviste: " + cantidadRecompensa1 + " " + nombre1 +
               ", " + cantidadRecompensa2 + " " + nombre2 +
               ", " + cantidadRecompensa3 + " " + nombre3;
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

            if (mostrarIconos && GameManager.Instancia != null)
            {
                if (iconoRecompensa1 != null)
                    iconoRecompensa1.sprite = GameManager.Instancia.ObtenerIconoDe(ingredienteRecompensa1);
                if (iconoRecompensa2 != null)
                    iconoRecompensa2.sprite = GameManager.Instancia.ObtenerIconoDe(ingredienteRecompensa2);
                if (iconoRecompensa3 != null)
                    iconoRecompensa3.sprite = GameManager.Instancia.ObtenerIconoDe(ingredienteRecompensa3);

                if (filaIconosRecompensa != null)
                    filaIconosRecompensa.SetActive(true);

                if (textoSinRecompensa != null)
                    textoSinRecompensa.SetActive(false);
            }
            else
            {
                if (filaIconosRecompensa != null)
                    filaIconosRecompensa.SetActive(false);

                if (textoSinRecompensa != null)
                    textoSinRecompensa.SetActive(true);
            }

            if (botonContinuar != null)
            {
                botonContinuar.onClick.RemoveAllListeners();
                botonContinuar.onClick.AddListener(VolverAlMapa);
            }

            if (imagenBotonContinuar != null)
            {
                imagenBotonContinuar.sprite = mostrarIconos ? spriteBotonSiguiente : spriteBotonReintentar;
            }
        }
    }

    private void VolverAlMapa()
    {
        if (InventarioUI.Instancia != null)
            InventarioUI.Instancia.Mostrar(true);

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