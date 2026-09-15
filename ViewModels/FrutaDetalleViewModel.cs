using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MiPrimeraApp.Models;

namespace MiPrimeraApp.ViewModels;

public class FrutaDetalleViewModel : INotifyPropertyChanged
{
    private Fruta? _fruta;
    private string _nuevaDescripcion = string.Empty;
    private string _mensajeEstado = string.Empty;

    public Fruta? Fruta
    {
        get => _fruta;
        set
        {
            if (SetProperty(ref _fruta, value))
            {
                // Al asignar la fruta, inicializamos el editor con su descripción actual
                NuevaDescripcion = _fruta?.Descripcion ?? string.Empty;
                OnPropertyChanged(nameof(Nombre));
                OnPropertyChanged(nameof(Emoji));
                OnPropertyChanged(nameof(ColorHex));
                OnPropertyChanged(nameof(DescripcionActual));
            }
        }
    }

    // Propiedades de conveniencia para binding directo
    public string Nombre => Fruta?.Nombre ?? string.Empty;
    public string Emoji => Fruta?.Emoji ?? "🍎";
    public string ColorHex => Fruta?.ColorHex ?? "#FF6B6B";
    public string DescripcionActual => Fruta?.Descripcion ?? string.Empty;

    public string NuevaDescripcion
    {
        get => _nuevaDescripcion;
        set
        {
            if (SetProperty(ref _nuevaDescripcion, value))
            {
                OnPropertyChanged(nameof(PuedeGuardar));
                (GuardarCommand as Command)?.ChangeCanExecute();
            }
        }
    }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        set => SetProperty(ref _mensajeEstado, value);
    }

    public bool PuedeGuardar => !string.IsNullOrWhiteSpace(NuevaDescripcion);

    public ICommand GuardarCommand { get; }
    public ICommand VolverCommand { get; }

    public FrutaDetalleViewModel()
    {
        GuardarCommand = new Command(async () => await GuardarDescripcionAsync(), () => PuedeGuardar);
        VolverCommand = new Command(async () => await VolverAsync());
    }

    private async Task GuardarDescripcionAsync()
    {
        if (Fruta == null)
            return;

        if (string.IsNullOrWhiteSpace(NuevaDescripcion))
        {
            MensajeEstado = "La descripción no puede estar vacía.";
            return;
        }

        // Actualizar el modelo -> dispara INotifyPropertyChanged
        // Se refleja automáticamente en esta pantalla (DescripcionActual)
        // y también en FrutasPage al volver (misma referencia)
        Fruta.Descripcion = NuevaDescripcion.Trim();

        // Notificar que la descripción actual cambió
        OnPropertyChanged(nameof(DescripcionActual));

        MensajeEstado = "¡Descripción actualizada!";

        // Feedback visual opcional
        await Shell.Current.DisplayAlertAsync("Guardado", $"Descripción de {Fruta.Nombre} actualizada correctamente.", "OK");

        // No hacemos pop automático para que se vea el cambio reflejado en el label de la misma pantalla
        // Si quieres volver automáticamente, descomenta:
        // await Shell.Current.GoToAsync("..");
    }

    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
