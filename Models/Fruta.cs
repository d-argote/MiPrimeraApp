using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiPrimeraApp.Models;

public class Fruta : INotifyPropertyChanged
{
    private string _nombre = string.Empty;
    private string _descripcion = string.Empty;
    private string _emoji = "🍎";
    private string _colorHex = "#FF6B6B";

    public string Nombre
    {
        get => _nombre;
        set => SetProperty(ref _nombre, value);
    }

    public string Descripcion
    {
        get => _descripcion;
        set => SetProperty(ref _descripcion, value);
    }

    public string Emoji
    {
        get => _emoji;
        set => SetProperty(ref _emoji, value);
    }

    public string ColorHex
    {
        get => _colorHex;
        set => SetProperty(ref _colorHex, value);
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
