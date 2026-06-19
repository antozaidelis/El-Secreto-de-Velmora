using UnityEngine;

public class TransicionPantalla : MonoBehaviour
{
    // La cámara que maneja la pantalla actual (la que querés apagar)
    public GameObject camaraActual;
    // La nueva cámara de Cinemachine que va a enfocar el siguiente fondo
    public GameObject camaraSiguiente;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el gatito cruza la puerta invisible
        if (other.CompareTag("Player"))
        {
            if (camaraActual != null && camaraSiguiente != null)
            {
                camaraActual.SetActive(false);     // Apaga la pantalla vieja
                camaraSiguiente.SetActive(true);   // Enciende la pantalla nueva
            }
        }
    }
}
