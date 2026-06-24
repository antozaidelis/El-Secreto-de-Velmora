using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// CAJA DE AHORRO EN MEMORIA (Sobrevive al cambio de escena)
public static class DatosInventarioCompartido
{
    public static List<SlotInventario> slotsGuardados = new List<SlotInventario>();
}

public class RecolectorIngredientes : MonoBehaviour
{
    [Header("Configuración del Inventario")]
    public int capacityMaxima = 12;
    public int maximoPorSlot = 5;

    // Vincula tu mochila a la memoria estática
    public List<SlotInventario> slotsInventario
    {
        get { return DatosInventarioCompartido.slotsGuardados; }
        set { DatosInventarioCompartido.slotsGuardados = value; }
    }

    [Header("UI")]
    public InventarioUI inventarioUI;

    [Header("Iconos")]
    public List<Sprite> iconosDisponibles;
    public List<string> nombresDeIconos;

    private Dictionary<string, Sprite> mapaIconos = new Dictionary<string, Sprite>();

    void Start()
    {
        ArmarMapaDeIconos();
        ActualizarUI();
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

    public bool AgregarIngrediente(string nombreIngrediente)
    {
        foreach (SlotInventario slot in slotsInventario)
        {
            if (slot.nombreIngrediente == nombreIngrediente && slot.cantidad < maximoPorSlot)
            {
                slot.cantidad++;
                ActualizarUI();
                return true;
            }
        }

        if (slotsInventario.Count < capacityMaxima)
        {
            slotsInventario.Add(new SlotInventario(nombreIngrediente, 1));
            ActualizarUI();
            return true;
        }

        return false;
    }

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

    public void ArmarMapaDeIconos()
    {
        mapaIconos.Clear();
        for (int i = 0; i < nombresDeIconos.Count; i++)
        {
            if (i < iconosDisponibles.Count)
                mapaIconos[nombresDeIconos[i]] = iconosDisponibles[i];
        }
    }

    public void ActualizarUI()
    {
        ArmarMapaDeIconos();

        if (inventarioUI == null) return;

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