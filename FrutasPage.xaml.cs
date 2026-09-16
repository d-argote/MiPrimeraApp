using MiPrimeraApp.Models;
using MiPrimeraApp.ViewModels;
using MiPrimeraApp.Views;

namespace MiPrimeraApp;

public partial class FrutasPage : ContentPage
{
    private readonly FrutaViewModel _viewModel;
    private bool _isNavigating = false;

    public FrutasPage()
    {
        InitializeComponent();

        // Usamos el ViewModel con CommunityToolkit que te pasaron.
        // Su colección está sincronizada con FrutaStore.Frutas (ver ViewModels/FrutaDetalleViewModel.cs:FrutaStore)
        _viewModel = new FrutaViewModel();

        // Hacemos que FrutaViewModel.Frutas sea la misma que FrutaStore.Frutas para que el detalle pueda editarla:
        // Si quieres mantenerlas separadas, quita estas líneas y cada VM tendrá su copia.
        // Pero para que "se vea reflejado en la lista al volver", deben compartir la misma instancia.
        if (_viewModel.Frutas.Count != FrutaStore.Frutas.Count)
        {
            // Sincronizar si difieren (solo la primera vez)
            _viewModel.Frutas.Clear();
            foreach (var f in FrutaStore.Frutas)
                _viewModel.Frutas.Add(f);
        }

        BindingContext = _viewModel;
        // Si usas x:DataType y binding ItemsSource="{Binding Frutas}", no necesitas la línea siguiente,
        // pero la dejamos por si el binding aún no aplica al crear la Page:
        frutasCollection.ItemsSource = _viewModel.Frutas;
    }

    private async void OnFrutaSeleccionada(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Console.WriteLine($"[FrutasPage] SelectionChanged count={e.CurrentSelection.Count}");
            if (e.CurrentSelection.FirstOrDefault() is Fruta fruta)
            {
                Console.WriteLine($"[FrutasPage] Seleccionada: {fruta.Nombre}");
                ((CollectionView)sender).SelectedItem = null;
                await NavegarViaViewModelAsync(fruta);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FrutasPage] ERROR SelectionChanged: {ex}");
            await DisplayAlertAsync("Error navegación", ex.ToString(), "OK");
        }
    }

    private async void OnFrutaTapped(object sender, TappedEventArgs e)
    {
        try
        {
            if ((sender as BindableObject)?.BindingContext is Fruta fruta)
            {
                Console.WriteLine($"[FrutasPage] Tap: {fruta.Nombre}");
                await NavegarViaViewModelAsync(fruta);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FrutasPage] ERROR Tap: {ex}");
            await DisplayAlertAsync("Error", ex.ToString(), "OK");
        }
    }

    private async void OnTestDetalleClicked(object sender, EventArgs e)
    {
        Console.WriteLine("[FrutasPage] TEST button clicked");
        if (_viewModel.Frutas.FirstOrDefault() is Fruta f)
            await NavegarViaViewModelAsync(f);
    }

    private async Task NavegarViaViewModelAsync(Fruta fruta)
    {
        if (_isNavigating) return;
        _isNavigating = true;
        try
        {
            // Delegamos al ViewModel que te pasaron (CommunityToolkit). Él hace el Shell.Current.GoToAsync
            // con los parámetros NombreFruta/DescripcionFruta + Fruta completa.
            if (_viewModel.IrADetalleCommand.CanExecute(fruta))
            {
                await _viewModel.IrADetalleCommand.ExecuteAsync(fruta);
            }
            else
            {
                // Fallback manual por si el comando está deshabilitado
                var parametros = new Dictionary<string, object>
                {
                    ["Fruta"] = fruta,
                    ["NombreFruta"] = fruta.Nombre,
                    ["DescripcionFruta"] = fruta.Descripcion,
                    ["Emoji"] = fruta.Emoji,
                    ["ColorHex"] = fruta.ColorHex
                };
                await Shell.Current.GoToAsync(nameof(FrutaDetallePage), parametros);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FrutasPage] Navegación vía ViewModel falló: {ex} -> fallback PushAsync");
            var page = new FrutaDetallePage();
            page.CargarFrutaDirecto(fruta);
            await Navigation.PushAsync(page);
        }
        finally
        {
            await Task.Delay(400);
            _isNavigating = false;
        }
    }
}
