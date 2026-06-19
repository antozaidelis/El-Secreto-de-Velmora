using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource[] audioSources;
    private AudioSource musicaExploracion;
    private AudioSource musicaCombate;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSources = GetComponents<AudioSource>();
        musicaExploracion = audioSources[0];
        musicaCombate = audioSources[1];
    }

    void Start()
    {
        IniciarExploracion();
    }

    public void IniciarCombate()
    {
        musicaExploracion.Stop();
        musicaCombate.Play();
    }

    public void IniciarExploracion()
    {
        musicaCombate.Stop();
        musicaExploracion.Play();
    }
}