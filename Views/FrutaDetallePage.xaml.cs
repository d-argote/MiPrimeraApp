using MiPrimeraApp.Models;
using MiPrimeraApp.ViewModels;

namespace MiPrimeraApp.Views;

// QueryProperty permite recibir el objeto Fruta via Shell navigation con Dictionary<string,object>
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

    // Esta propiedad es invocada automáticamente por Shell cuando navegas con:
    // await Shell.Current.GoToAsync("FrutaDetallePage", new Dictionary<string, object> { ["Fruta"] = fruta });
    public Fruta Fruta
    {
        set
        {
            if (value != null)
            {
                _viewModel.Fruta = value;
                // También sincronizar título si quieres
                Title = value.Nombre + " " + value.Emoji;
            }
        }
    }
}
