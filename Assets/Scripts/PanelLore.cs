using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelLore : MonoBehaviour
{
    public static PanelLore Instancia { get; private set; }

    [Header("Referencias")]
    public GameObject panel;
    public TextMeshProUGUI textoLore;
    public Button botonSiguiente;

    private string[] frasesActuales;
    private int indiceFraseActual = 0;

    void Awake()
    {
        Instancia = this;
        if (panel != null) panel.SetActive(false);

        if (botonSiguiente != null)
            botonSiguiente.onClick.AddListener(AvanzarFrase);
    }

    public void Mostrar(string[] frases)
    {
        if (frases == null || frases.Length == 0) return;

        frasesActuales = frases;
        indiceFraseActual = 0;

        if (panel != null) panel.SetActive(true);
        MostrarFraseActual();
    }

    private void MostrarFraseActual()
    {
        if (textoLore != null && frasesActuales != null && indiceFraseActual < frasesActuales.Length)
            textoLore.text = frasesActuales[indiceFraseActual];
    }

    private void AvanzarFrase()
    {
        indiceFraseActual++;

        if (frasesActuales != null && indiceFraseActual < frasesActuales.Length)
        {
            MostrarFraseActual();
        }
        else
        {
            Ocultar();
        }
    }

    public void Ocultar()
    {
        if (panel != null) panel.SetActive(false);
        frasesActuales = null;
        indiceFraseActual = 0;
    }

    public bool EstaAbierto()
    {
        return panel != null && panel.activeSelf;
    }
}
