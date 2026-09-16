using MiPrimeraApp.Models;
using MiPrimeraApp.ViewModels;

namespace MiPrimeraApp.Views;

// QueryProperty para soportar tanto el snippet original (NombreFruta/DescripcionFruta)
// como el objeto completo Fruta. Así funciona con el código que te pasaron y con el nuestro.
[QueryProperty(nameof(NombreFruta), "NombreFruta")]
[QueryProperty(nameof(DescripcionFruta), "DescripcionFruta")]
[QueryProperty(nameof(Emoji), "Emoji")]
[QueryProperty(nameof(ColorHex), "ColorHex")]
[QueryProperty(nameof(Fruta), "Fruta")]
public partial class FrutaDetallePage : ContentPage
{
    private readonly FrutaDetalleViewModel _viewModel;

    public FrutaDetallePage()
    {
        InitializeComponent();
        _viewModel = new FrutaDetalleViewModel();
        BindingContext = _viewModel;
    }

    // Estos setters son invocados por Shell.GoToAsync con Dictionary<string,object>
    public string NombreFruta
    {
        set => _viewModel.NombreFruta = value;
    }

    public string DescripcionFruta
    {
        set => _viewModel.DescripcionFruta = value;
    }

    public string Emoji
    {
        set => _viewModel.Emoji = value;
    }

    public string ColorHex
    {
        set => _viewModel.ColorHex = value;
    }

    // Objeto completo: si viene, inicializa todo el ViewModel de golpe
    public Fruta? Fruta
    {
        get => null; // solo setter necesario para QueryProperty
        set
        {
            if (value != null)
            {
                Console.WriteLine($"[FrutaDetallePage] Recibida Fruta objeto: {value.Nombre}");
                _viewModel.CargarDesdeFruta(value);
                Title = value.Nombre + " " + value.Emoji;
            }
        }
    }

    // Helper para navegación vía PushAsync (fallback)
    public void CargarFrutaDirecto(Fruta fruta)
    {
        _viewModel.CargarDesdeFruta(fruta);
        Title = fruta.Nombre + " " + fruta.Emoji;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine($"[FrutaDetallePage] OnAppearing: { _viewModel.NombreFruta} / {_viewModel.DescripcionFruta}");
        // Si venimos por strings, asegurar que NuevaDescripcion tenga valor inicial
        if (string.IsNullOrEmpty(_viewModel.NuevaDescripcion) && !string.IsNullOrEmpty(_viewModel.DescripcionFruta))
        {
            _viewModel.NuevaDescripcion = _viewModel.DescripcionFruta;
        }
        // Si no hay título aún, usar NombreFruta
        if (string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(_viewModel.NombreFruta))
            Title = _viewModel.NombreFruta + " " + _viewModel.Emoji;
    }
}

// Alias para que el snippet que usa DetalleFrutaPage no falle.
// DetalleFrutaPage es lo mismo que FrutaDetallePage, solo cambia el nombre.
public class DetalleFrutaPage : FrutaDetallePage { }
