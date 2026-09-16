using MiPrimeraApp.Models;
using MiPrimeraApp.Views;

namespace MiPrimeraApp;

public partial class FrutasPage : ContentPage
{
    public List<Fruta> Frutas { get; set; }
    private bool _isNavigating = false;

    public FrutasPage()
    {
        InitializeComponent();

        // Lista de frutas con nombre y descripción
        Frutas = new List<Fruta>
        {
            new Fruta { Nombre = "Manzana", Descripcion = "Fruta crujiente y jugosa, rica en fibra y vitamina C. Ideal para comer fresca o en postres.", Emoji = "🍎", ColorHex = "#FF6B6B" },
            new Fruta { Nombre = "Plátano", Descripcion = "Fruta tropical dulce y cremosa, excelente fuente de potasio y energía natural.", Emoji = "🍌", ColorHex = "#FFD93D" },
            new Fruta { Nombre = "Naranja", Descripcion = "Cítrico jugoso y refrescante, lleno de vitamina C y antioxidantes.", Emoji = "🍊", ColorHex = "#FF9F1C" },
            new Fruta { Nombre = "Fresa", Descripcion = "Pequeña fruta roja, dulce y aromática. Perfecta para batidos y postres.", Emoji = "🍓", ColorHex = "#FF4D6D" },
            new Fruta { Nombre = "Uva", Descripcion = "Racimos de bayas dulces, se consumen frescas o se usan para hacer vino y jugos.", Emoji = "🍇", ColorHex = "#9D4EDD" },
            new Fruta { Nombre = "Piña", Descripcion = "Fruta tropical de pulpa amarilla, ácida y dulce. Muy refrescante y diurética.", Emoji = "🍍", ColorHex = "#FFB703" },
            new Fruta { Nombre = "Sandía", Descripcion = "Fruta grande y jugosa con alto contenido de agua. Perfecta para hidratarse en verano.", Emoji = "🍉", ColorHex = "#06D6A0" },
            new Fruta { Nombre = "Kiwi", Descripcion = "Fruta exótica de pulpa verde y sabor agridulce, muy rica en vitamina C.", Emoji = "🥝", ColorHex = "#8BC34A" },
            new Fruta { Nombre = "Mango", Descripcion = "El rey de las frutas tropicales, pulpa dulce, jugosa y aromática.", Emoji = "🥭", ColorHex = "#FB8500" },
            new Fruta { Nombre = "Cereza", Descripcion = "Pequeña fruta roja y brillante, dulce con un toque ácido. Muy apreciada en repostería.", Emoji = "🍒", ColorHex = "#E63946" },
            new Fruta { Nombre = "Limón", Descripcion = "Cítrico ácido y aromático, indispensable en cocina y bebidas. Rico en vitamina C.", Emoji = "🍋", ColorHex = "#FDE74C" },
            new Fruta { Nombre = "Melocotón", Descripcion = "Fruta de piel aterciopelada y pulpa jugosa, dulce y perfumada.", Emoji = "🍑", ColorHex = "#FF8FA3" },
        };

        frutasCollection.ItemsSource = Frutas;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine("[FrutasPage] OnAppearing");
        // Auto-navegación de prueba: si en 3 segundos no se ha navegado, navegar automáticamente a la primera fruta para probar Shell
        // Esto ayuda a diagnosticar si el problema es el tap o la navegación
        await Task.Delay(3000);
        if (! _isNavigating && Frutas.Count > 0)
        {
            Console.WriteLine("[FrutasPage] Auto-navegación de prueba a " + Frutas[0].Nombre);
            // Solo si sigue en esta página (evitar doble)
            if (Shell.Current.CurrentPage == this || Navigation.NavigationStack.LastOrDefault() == this)
            {
                await NavegarADetalleAsync(Frutas[0]);
            }
        }
    }

    private async void OnFrutaSeleccionada(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            Console.WriteLine($"[FrutasPage] SelectionChanged fired, count={e.CurrentSelection.Count}");
            if (e.CurrentSelection.FirstOrDefault() is Fruta fruta)
            {
                Console.WriteLine($"[FrutasPage] Fruta seleccionada via SelectionChanged: {fruta.Nombre}");
                // Deseleccionar visualmente antes de navegar
                ((CollectionView)sender).SelectedItem = null;
                await NavegarADetalleAsync(fruta);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FrutasPage] ERROR navegación SelectionChanged: {ex}");
            await DisplayAlertAsync("Error navegación", ex.ToString(), "OK");
        }
    }

    private async void OnFrutaTapped(object sender, TappedEventArgs e)
    {
        try
        {
            // El sender es el Frame, su BindingContext es la Fruta
            if ((sender as BindableObject)?.BindingContext is Fruta fruta)
            {
                Console.WriteLine($"[FrutasPage] Fruta tapeada via TapGesture: {fruta.Nombre}");
                await NavegarADetalleAsync(fruta);
            }
            else if (e.Parameter is Fruta frutaParam)
            {
                await NavegarADetalleAsync(frutaParam);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FrutasPage] ERROR navegación Tap: {ex}");
            await DisplayAlertAsync("Error navegación", ex.ToString(), "OK");
        }
    }

    private async Task NavegarADetalleAsync(Fruta fruta)
    {
        if (_isNavigating) return;
        _isNavigating = true;
        try
        {
            var parametros = new Dictionary<string, object>
            {
                ["Fruta"] = fruta
            };
            Console.WriteLine($"[FrutasPage] Navegando a {nameof(FrutaDetallePage)} via Shell con fruta {fruta.Nombre}...");
            try
            {
                await Shell.Current.GoToAsync(nameof(FrutaDetallePage), parametros);
                Console.WriteLine($"[FrutasPage] Shell.GoToAsync OK");
            }
            catch (Exception shellEx)
            {
                Console.WriteLine($"[FrutasPage] Shell.GoToAsync FALLÓ: {shellEx} -> fallback Navigation.PushAsync");
                // Fallback por si la ruta de Shell falla (ej. TabBar)
                var page = new FrutaDetallePage();
                page.Fruta = fruta;
                await Navigation.PushAsync(page);
                Console.WriteLine($"[FrutasPage] fallback PushAsync OK");
            }
        }
        finally
        {
            // Pequeño delay para evitar doble tap
            await Task.Delay(500);
            _isNavigating = false;
        }
    }
}
