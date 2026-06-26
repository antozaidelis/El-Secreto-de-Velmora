using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GestorTransicion : MonoBehaviour
{
    public static GestorTransicion Instancia { get; private set; }

    public Image panelNegro;
    public float duracionFade = 0.4f;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);

        if (panelNegro != null)
        {
            Color c = panelNegro.color;
            c.a = 0f;
            panelNegro.color = c;
        }
    }

    public void CambiarEscenaConFade(string nombreEscena)
    {
        StartCoroutine(RutinaCambioEscena(nombreEscena));
    }

    private IEnumerator RutinaCambioEscena(string nombreEscena)
    {
        yield return StartCoroutine(Fade(0f, 1f));

        SceneManager.LoadScene(nombreEscena);

        yield return null;

        yield return StartCoroutine(Fade(1f, 0f));
    }

    public void TransicionEntrePaneles(System.Action accionEnElMedio)
    {
        StartCoroutine(RutinaTransicionPaneles(accionEnElMedio));
    }

    private IEnumerator RutinaTransicionPaneles(System.Action accionEnElMedio)
    {
        yield return StartCoroutine(Fade(0f, 1f));

        accionEnElMedio?.Invoke();

        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float desde, float hasta)
    {
        if (panelNegro == null) yield break;

        float tiempoTranscurrido = 0f;
        Color c = panelNegro.color;

        while (tiempoTranscurrido < duracionFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = Mathf.Clamp01(tiempoTranscurrido / duracionFade);
            float tSuavizado = Mathf.SmoothStep(0f, 1f, t);
            c.a = Mathf.Lerp(desde, hasta, tSuavizado);
            panelNegro.color = c;
            yield return null;
        }

        c.a = hasta;
        panelNegro.color = c;
    }
}
