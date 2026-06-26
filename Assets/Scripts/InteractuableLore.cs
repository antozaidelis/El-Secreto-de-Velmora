using UnityEngine;

public class InteractuableLore : MonoBehaviour
{
    [Header("Referencias (Arrastra aquí los objetos)")]
    public GameObject indicador; // Arrastra el objeto "IndicadorInteraccion" de la jerarquía aquí
    public PanelLore panelLore;  // Arrastra el objeto "PanelLore" de la jerarquía aquí

    [Header("Configuración")]
    public KeyCode teclaInteractuar = KeyCode.E;
    public Vector3 offsetIndicador = new Vector3(0f, 1f, 0f);

    [TextArea(2, 5)]
    public string[] frasesLore;

    private bool jugadorCerca = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
            indicador.SetActive(false); // Ocultar
        }
    }

    void Update()
    {
        if (jugadorCerca)
        {
            // Mostramos indicador si el panel NO está activo
            bool panelActivo = panelLore.gameObject.activeSelf;
            indicador.SetActive(!panelActivo);

            if (!panelActivo && Input.GetKeyDown(teclaInteractuar))
            {
                indicador.SetActive(false);
                panelLore.Mostrar(frasesLore);
            }
        }
    }
}