using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Representa un slot del inventario: qué ingrediente tiene y cuántos
[System.Serializable]
public class SlotInventario
{
    public string nombreIngrediente;
    public int cantidad;

    public SlotInventario(string nombre, int cant)
    {
        nombreIngrediente = nombre;
        cantidad = cant;
    }
}

public class RecolectorIngredientes : MonoBehaviour
{
    [Header("Configuración del Inventario")]
    public int capacityMaxima = 12;
    public int maximoPorSlot = 5;

    // Esta es la nueva mochila: una lista de slots, cada uno con ingrediente + cantidad
    public List<SlotInventario> slotsInventario = new List<SlotInventario>();

    [Header("UI")]
    public InventarioUI inventarioUI;

    [Header("Iconos")]
    public List<Sprite> iconosDisponibles;
    public List<string> nombresDeIconos;

    private Dictionary<string, Sprite> mapaIconos = new Dictionary<string, Sprite>();

    void Start()
    {
        for (int i = 0; i < nombresDeIconos.Count; i++)
        {
            if (i < iconosDisponibles.Count)
                mapaIconos[nombresDeIconos[i]] = iconosDisponibles[i];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ingrediente"))
        {
            string nombreLimpio = collision.gameObject.name.Split('(')[0].Trim();
            AgregarIngrediente(nombreLimpio);
            Destroy(collision.gameObject);
        }
    }

    // ---------- LÓGICA DE STACK ----------

    public bool AgregarIngrediente(string nombreIngrediente)
    {
        // 1. Buscar si ya hay un slot con este ingrediente que NO esté lleno
        foreach (SlotInventario slot in slotsInventario)
        {
            if (slot.nombreIngrediente == nombreIngrediente && slot.cantidad < maximoPorSlot)
            {
                slot.cantidad++;
                Debug.Log("Sumaste " + nombreIngrediente + ". Ahora tenés " + slot.cantidad + " en ese slot.");
                ActualizarUI();
                return true;
            }
        }

        // 2. Si no hay slot disponible para ese ingrediente, abrir uno nuevo (si hay espacio)
        if (slotsInventario.Count < capacityMaxima)
        {
            slotsInventario.Add(new SlotInventario(nombreIngrediente, 1));
            Debug.Log("Nuevo slot abierto para " + nombreIngrediente);
            ActualizarUI();
            return true;
        }

        Debug.Log("¡Mochila llena! No se puede agregar " + nombreIngrediente);
        return false;
    }

    // Gasta 1 unidad de un ingrediente específico. Devuelve true si pudo gastarlo.
    public bool GastarIngrediente(string nombreIngrediente)
    {
        foreach (SlotInventario slot in slotsInventario)
        {
            if (slot.nombreIngrediente == nombreIngrediente && slot.cantidad > 0)
            {
                slot.cantidad--;

                if (slot.cantidad <= 0)
                    slotsInventario.Remove(slot);

                ActualizarUI();
                return true;
            }
        }
        return false;
    }

    // Cuenta cuántas unidades totales hay de un ingrediente (sumando todos sus slots)
    public int ContarIngrediente(string nombreIngrediente)
    {
        int total = 0;
        foreach (SlotInventario slot in slotsInventario)
        {
            if (slot.nombreIngrediente == nombreIngrediente)
                total += slot.cantidad;
        }
        return total;
    }

    // Devuelve el sprite asociado a un nombre de ingrediente (usado por el panel de selección en combate)
    // Tolerante a diferencias de mayúsculas/minúsculas y espacios extra al inicio/final
    public Sprite ObtenerIconoDe(string nombreIngrediente)
    {
        if (string.IsNullOrEmpty(nombreIngrediente)) return null;

        string buscado = nombreIngrediente.Trim().ToLower();

        foreach (var par in mapaIconos)
        {
            if (par.Key.Trim().ToLower() == buscado)
                return par.Value;
        }

        return null;
    }

    // ---------- UI ----------

    void ActualizarUI()
    {
        List<Sprite> iconos = new List<Sprite>();
        List<int> cantidades = new List<int>();

        foreach (SlotInventario slot in slotsInventario)
        {
            Sprite icono = ObtenerIconoDe(slot.nombreIngrediente);
            if (icono != null)
            {
                iconos.Add(icono);
                cantidades.Add(slot.cantidad);
            }
        }

        inventarioUI.ActualizarUI(iconos, cantidades);
    }
}