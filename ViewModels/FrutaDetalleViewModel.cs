using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiPrimeraApp.Models;

namespace MiPrimeraApp.ViewModels;

/// <summary>
/// ViewModel para el DETALLE de fruta (CommunityToolkit.Mvvm).
/// - Recibe NombreFruta/DescripcionFruta (+ Emoji/ColorHex y opcionalmente el objeto Fruta completo) vía Shell.
/// - Permite editar la descripción con NuevaDescripcion.
/// - Al guardar, actualiza DescripcionFruta (visible en esta pantalla) Y también la colección original
///   para que al volver a la lista se vea el cambio.
/// - Además expone ObservableCollection Frutas con TODAS las frutas si quieres "llamar a todas"
///   desde el detalle (ej. para mostrar un picker o la lista completa).
/// </summary>
public partial class FrutaDetalleViewModel : ObservableObject
{
    // Propiedades que vienen de la navegación (QueryProperty)
    [ObservableProperty]
    private string nombreFruta = string.Empty;

    [ObservableProperty]
    private string descripcionFruta = string.Empty;

    [ObservableProperty]
    private string emoji = "🍎";

    [ObservableProperty]
    private string colorHex = "#FF6B6B";

    // Editor
    [ObservableProperty]
    private string nuevaDescripcion = string.Empty;

    [ObservableProperty]
    private string mensajeEstado = string.Empty;

    // Referencia al objeto original de la lista (si se pasó "Fruta").
    // Si este campo no es null, al guardar editamos directamente ese objeto,
    // y gracias a INotifyPropertyChanged de Fruta, la lista se actualiza sola.
    private Fruta? _frutaOriginal;

    // Para que el detalle pueda "llamar a todas las frutas" como pedías:
    // Exponemos la misma colección que usa FrutaViewModel.
    // Opción 1: colección independiente (copia) – no se sincroniza
    // Opción 2: referencia a singleton (ver FrutaStore más abajo) – sí se sincroniza
    // Aquí usamos la opción 2: misma instancia que la lista.
    public ObservableCollection<Fruta> Frutas => FrutaStore.Frutas;

    // Propiedades de conveniencia para el XAML
    public string DescripcionActual => DescripcionFruta;
    public string Nombre => NombreFruta;

    // El Toolkit genera automáticamente:
    // - NombreFruta, DescripcionFruta, NuevaDescripcion, MensajeEstado, Emoji, ColorHex con OnPropertyChanged
    // Pero necesitamos lógica extra cuando cambian: si DescripcionFruta cambia, avisar que DescripcionActual cambió.

    partial void OnDescripcionFrutaChanged(string value)
    {
        OnPropertyChanged(nameof(DescripcionActual));
    }

    partial void OnNombreFrutaChanged(string value)
    {
        OnPropertyChanged(nameof(Nombre));
    }

    // Este método es invocado si la Page usa [QueryProperty] con Dictionary<string,object>
    // CommunityToolkit no lo hace solo; la Page debe reenviar. Pero si usas IQueryAttributable en el VM,
    // Shell lo invoca automáticamente si el BindingContext es el VM. Para simplicidad, la Page hará el reenvío.
    public void CargarDesdeFruta(Fruta fruta)
    {
        _frutaOriginal = fruta;
        NombreFruta = fruta.Nombre;
        DescripcionFruta = fruta.Descripcion;
        Emoji = fruta.Emoji;
        ColorHex = fruta.ColorHex;
        NuevaDescripcion = fruta.Descripcion;
        MensajeEstado = string.Empty;
        Console.WriteLine($"[FrutaDetalleViewModel] Cargado desde Fruta: {fruta.Nombre}");
    }

    public void CargarDesdeParametros(string nombre, string descripcion, string? emoji = null, string? colorHex = null, Fruta? frutaRef = null)
    {
        if (frutaRef != null)
        {
            CargarDesdeFruta(frutaRef);
            return;
        }
        NombreFruta = nombre;
        DescripcionFruta = descripcion;
        if (emoji != null) Emoji = emoji;
        if (colorHex != null) ColorHex = colorHex;
        NuevaDescripcion = descripcion;
        MensajeEstado = string.Empty;
        _frutaOriginal = null;
        Console.WriteLine($"[FrutaDetalleViewModel] Cargado desde strings: {nombre}");
    }

    [RelayCommand(CanExecute = nameof(CanGuardar))]
    async Task Guardar()
    {
        if (string.IsNullOrWhiteSpace(NuevaDescripcion))
        {
            MensajeEstado = "La descripción no puede estar vacía.";
            return;
        }

        var nueva = NuevaDescripcion.Trim();
        Console.WriteLine($"[FrutaDetalleViewModel] Guardar: {NombreFruta} nueva desc: {nueva}");

        // 1. Actualizar la propiedad local (se refleja en esta pantalla)
        DescripcionFruta = nueva;
        NuevaDescripcion = nueva; // normalizar

        // 2. Si tenemos referencia al objeto original, actualizarlo también (se refleja en la lista)
        if (_frutaOriginal != null)
        {
            _frutaOriginal.Descripcion = nueva;
            Console.WriteLine($"[FrutaDetalleViewModel] Actualizado objeto original en colección");
        }
        else
        {
            // Si no hay referencia, buscar en la colección global por nombre y actualizar
            var existente = Frutas.FirstOrDefault(f => f.Nombre == NombreFruta);
            if (existente != null)
            {
                existente.Descripcion = nueva;
                Console.WriteLine($"[FrutaDetalleViewModel] Actualizado en Frutas global: {existente.Nombre}");
            }
            else
            {
                Console.WriteLine($"[FrutaDetalleViewModel] No se encontró en colección global, solo se actualizó vista local");
            }
        }

        MensajeEstado = "¡Descripción actualizada!";
        await Shell.Current.DisplayAlertAsync("Guardado", $"Descripción de {NombreFruta} actualizada.", "OK");
    }

    bool CanGuardar() => !string.IsNullOrWhiteSpace(NuevaDescripcion);

    partial void OnNuevaDescripcionChanged(string value)
    {
        GuardarCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    async Task Volver()
    {
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
                await Shell.Current.Navigation.PopAsync();
        }
    }
}

/// <summary>
/// Store singleton para que FrutaViewModel y FrutaDetalleViewModel compartan la misma colección.
/// Así, cuando el detalle edita una fruta, la lista lo ve al instante.
/// Si tu profesor quiere que cada ViewModel tenga su propia lista, puedes hacer que FrutaDetalleViewModel.Frutas
/// sea una nueva ObservableCollection en lugar de referenciar esta.
/// </summary>
public static class FrutaStore
{
    public static ObservableCollection<Fruta> Frutas { get; } = new()
    {
        new Fruta { Nombre = "Manzana", Descripcion = "Fruta roja o verde, crujiente y dulce.", Emoji = "🍎", ColorHex = "#FF6B6B" },
        new Fruta { Nombre = "Banano",  Descripcion = "Fruta amarilla, rica en potasio.", Emoji = "🍌", ColorHex = "#FFD93D" },
        new Fruta { Nombre = "Naranja", Descripcion = "Cítrico jugoso, alto en vitamina C.", Emoji = "🍊", ColorHex = "#FF9F1C" },
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
}
