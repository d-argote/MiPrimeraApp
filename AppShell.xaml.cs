using MiPrimeraApp.Views;

namespace MiPrimeraApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Registro de ruta para navegación con Shell
		// Permite hacer: await Shell.Current.GoToAsync(nameof(FrutaDetallePage), parametros)
		Routing.RegisterRoute(nameof(FrutaDetallePage), typeof(FrutaDetallePage));
	}
}
