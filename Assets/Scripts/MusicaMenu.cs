using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaMenu : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // Hacemos que este objeto no se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);

        // Nos suscribimos al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si la escena que se cargó es la principal, detenemos la música
        if (scene.name == "Escena_Lara")
        {
            StartCoroutine(FadeOutMusica());
        }
    }

    private System.Collections.IEnumerator FadeOutMusica()
    {
        float duracion = 1.0f;
        float volumenInicial = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= volumenInicial * Time.deltaTime / duracion;
            yield return null;
        }

        audioSource.Stop();
        // Una vez que terminó, eliminamos este objeto para que no ocupe memoria
        Destroy(gameObject);
    }
}