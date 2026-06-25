using UnityEngine;

public class NieblaDeGuerra : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    public SpriteRenderer spriteNiebla;

    [Header("Límites del mapa (mundo)")]
    public float mapaMinX = -50f;
    public float mapaMaxX = 80f;
    public float mapaMinY = -40f;
    public float mapaMaxY = 25f;

    [Header("Configuración de la textura")]
    public int resolucionAncho = 256;
    public int resolucionAlto = 128;

    [Header("Configuración del revelado")]
    public float radioRevelado = 15f; // en píxeles de textura
    public float velocidadActualizacion = 0.1f; // segundos entre actualizaciones

    private Texture2D textura;
    private Color32[] pixeles;
    private float temporizador = 0f;

    void Start()
    {
        CrearTextura();
    }

    private void CrearTextura()
    {
        textura = new Texture2D(resolucionAncho, resolucionAlto, TextureFormat.RGBA32, false);
        textura.filterMode = FilterMode.Bilinear;
        textura.wrapMode = TextureWrapMode.Clamp;

        pixeles = new Color32[resolucionAncho * resolucionAlto];
        for (int i = 0; i < pixeles.Length; i++)
            pixeles[i] = new Color32(0, 0, 0, 255); // negro opaco

        textura.SetPixels32(pixeles);
        textura.Apply();

        Sprite nuevoSprite = Sprite.Create(
            textura,
            new Rect(0, 0, resolucionAncho, resolucionAlto),
            new Vector2(0.5f, 0.5f)
        );

        spriteNiebla.sprite = nuevoSprite;

        // Posicionar y escalar el sprite para que cubra exactamente el mapa
        float anchoMundo = mapaMaxX - mapaMinX;
        float altoMundo = mapaMaxY - mapaMinY;

        transform.position = new Vector3(
            (mapaMinX + mapaMaxX) / 2f,
            (mapaMinY + mapaMaxY) / 2f,
            transform.position.z
        );

        float escalaX = anchoMundo / (resolucionAncho / 100f);
        float escalaY = altoMundo / (resolucionAlto / 100f);
        transform.localScale = new Vector3(escalaX / anchoMundo * anchoMundo / resolucionAncho * 100f, escalaY, 1f);

        // Cálculo simplificado de escala (ver nota abajo)
        spriteNiebla.transform.localScale = new Vector3(anchoMundo / resolucionAncho * 100f, altoMundo / resolucionAlto * 100f, 1f);
    }

    void Update()
    {
        if (jugador == null) return;

        temporizador += Time.deltaTime;
        if (temporizador < velocidadActualizacion) return;
        temporizador = 0f;

        RevelarAlrededorDeJugador();
    }

    private void RevelarAlrededorDeJugador()
    {
        Vector2 posMundo = jugador.position;

        float porcentajeX = Mathf.InverseLerp(mapaMinX, mapaMaxX, posMundo.x);
        float porcentajeY = Mathf.InverseLerp(mapaMinY, mapaMaxY, posMundo.y);

        int centroX = Mathf.RoundToInt(porcentajeX * resolucionAncho);
        int centroY = Mathf.RoundToInt(porcentajeY * resolucionAlto);

        int radio = Mathf.RoundToInt(radioRevelado);
        bool cambios = false;

        for (int x = centroX - radio; x <= centroX + radio; x++)
        {
            for (int y = centroY - radio; y <= centroY + radio; y++)
            {
                if (x < 0 || x >= resolucionAncho || y < 0 || y >= resolucionAlto) continue;

                float distancia = Vector2.Distance(new Vector2(x, y), new Vector2(centroX, centroY));
                if (distancia <= radio)
                {
                    int indice = y * resolucionAncho + x;

                    float factor = Mathf.Clamp01(distancia / radio);
                    byte nuevoAlpha = (byte)Mathf.Lerp(0f, 255f, factor);

                    if (pixeles[indice].a > nuevoAlpha)
                    {
                        pixeles[indice].a = nuevoAlpha;
                        cambios = true;
                    }
                }
            }
        }

        if (cambios)
        {
            textura.SetPixels32(pixeles);
            textura.Apply();
        }
    }
}
