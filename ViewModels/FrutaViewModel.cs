using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiPrimeraApp.Models;
using MiPrimeraApp.Views;

namespace MiPrimeraApp.ViewModels;

/// <summary>
/// ViewModel para la LISTA de frutas.
/// - Expone ObservableCollection Frutas (para que la UI se actualice automáticamente)
/// - Expone FrutaSeleccionada (bindable)
/// - Comando IrADetalle que navega vía AppShell al detalle
/// Este es EXACTAMENTE el código que te pasaron, corregido y ampliado con Emoji/ColorHex
/// y con navegación compatible con FrutaDetallePage y DetalleFrutaPage.
/// </summary>
public partial class FrutaViewModel : ObservableObject
{
    // Colección observable: cualquier cambio (añadir/editar propiedad de Fruta) se refleja en la UI
    // porque Fruta implementa INotifyPropertyChanged y la Collection es Observable
    public ObservableCollection<Fruta> Frutas { get; } = new()
    {
        new Fruta { Nombre = "Manzana", Descripcion = "Fruta roja o verde, crujiente y dulce.", Emoji = "🍎", ColorHex = "#FF6B6B" },
        new Fruta { Nombre = "Banano",  Descripcion = "Fruta amarilla, rica en potasio.", Emoji = "🍌", ColorHex = "#FFD93D" },
        new Fruta { Nombre = "Naranja", Descripcion = "Cítrico jugoso, alto en vitamina C.", Emoji = "🍊", ColorHex = "#FF9F1C" },
        // Las mismas 12 que tenías antes para que no pierdas datos (puedes dejar solo 3 si tu profe lo exige)
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

    // Esto genera una propiedad pública FrutaSeleccionada con INotifyPropertyChanged
    // Gracias a [ObservableProperty] del Toolkit
    [ObservableProperty]
    private Fruta? frutaSeleccionada;

    // Este es el comando que el profesor te pasó.
    // Lo corregimos para que navegue a FrutaDetallePage (que sí existe) y para que
    // pase TAMBIÉN el objeto completo "Fruta" (además de los strings) para que la edición se refleje en la lista.
    // Si tu profesor evalúa estrictamente que debe pasar NombreFruta/DescripcionFruta, eso se mantiene.
    [RelayCommand]
    async Task IrADetalle(Fruta? fruta)
    {
        if (fruta is null)
            return;

        Console.WriteLine($"[FrutaViewModel] IrADetalle: {fruta.Nombre}");

        var parametros = new Dictionary<string, object>
        {
            // Parámetros que pide el snippet original (string)
            { "NombreFruta", fruta.Nombre },
            { "DescripcionFruta", fruta.Descripcion },
            { "Emoji", fruta.Emoji },
            { "ColorHex", fruta.ColorHex },
            // Parámetro extra: el objeto completo para que el detalle pueda editar la misma referencia
            // y el cambio se vea en la lista automáticamente (sin esto, solo se editaría una copia string)
            { "Fruta", fruta }
        };

        // El snippet usaba nameof(DetalleFrutaPage) pero tu proyecto tiene FrutaDetallePage.
        // Registramos ambas rutas en AppShell, así funciona con cualquiera.
        try
        {
            await Shell.Current.GoToAsync(nameof(FrutaDetallePage), parametros);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FrutaViewModel] Shell.GoToAsync a FrutaDetallePage falló: {ex} -> probando DetalleFrutaPage");
            await Shell.Current.GoToAsync(nameof(DetalleFrutaPage), parametros);
        }
    }

    // Alternativa: comando sin parámetro que usa FrutaSeleccionada (útil si bindeas SelectedItem)
    [RelayCommand]
    async Task IrADetalleSeleccionada()
    {
        if (FrutaSeleccionada is null) return;
        await IrADetalle(FrutaSeleccionada);
    }
}
