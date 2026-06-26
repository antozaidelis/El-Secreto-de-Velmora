using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public string escenaJuego = "Nivel1"; // Escribe aquí el nombre exacto de tu escena de juego

    public void Jugar()
    {
        if (GestorTransicion.Instancia != null)
        {
            GestorTransicion.Instancia.CambiarEscenaConFade(escenaJuego);
        }
        else
        {
            SceneManager.LoadScene(escenaJuego);
        }
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo...");
    }
}