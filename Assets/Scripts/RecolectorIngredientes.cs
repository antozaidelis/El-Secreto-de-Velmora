using UnityEngine;

public class RecolectorIngredientes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ingrediente"))
        {
            string nombreLimpio = collision.gameObject.name.Split('(')[0].Trim();

            if (GameManager.Instancia != null)
                GameManager.Instancia.AgregarIngrediente(nombreLimpio);

            Destroy(collision.gameObject);
        }
    }
}