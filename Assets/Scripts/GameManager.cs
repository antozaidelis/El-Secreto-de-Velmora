using UnityEngine;
using System.Collections.Generic;

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

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Configuración del Inventario")]
    public int capacidadMaxima = 12;
    public int maximoPorSlot = 5;

    [Header("Iconos")]
    public List<Sprite> iconosDisponibles;
    public List<string> nombresDeIconos;

    public List<SlotInventario> slotsInventario = new List<SlotInventario>();
    public bool huronDerrotado = false;

    private Dictionary<string, Sprite> mapaIconos = new Dictionary<string, Sprite>();

    public event System.Action OnInventarioCambiado;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
        ArmarMapaDeIconos();
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

    public bool AgregarIngrediente(string nombreIngrediente)
    {
        foreach (SlotInventario slot in slotsInventario)
        {
            if (slot.nombreIngrediente == nombreIngrediente && slot.cantidad < maximoPorSlot)
            {
                slot.cantidad++;
                AvisarCambio();
                return true;
            }
        }

        if (slotsInventario.Count < capacidadMaxima)
        {
            slotsInventario.Add(new SlotInventario(nombreIngrediente, 1));
            AvisarCambio();
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
                AvisarCambio();
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

    public void VaciarInventario()
    {
        slotsInventario.Clear();
        AvisarCambio();
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

    private void AvisarCambio()
    {
        OnInventarioCambiado?.Invoke();
    }
}
