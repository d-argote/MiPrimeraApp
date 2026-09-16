using MiPrimeraApp.Views;

namespace MiPrimeraApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Registro de ruta para navegación con Shell
		// El código que te pasaron navega a DetalleFrutaPage, pero tu proyecto tiene FrutaDetallePage.
		// Registramos ambas para que funcionen las dos:
		Routing.RegisterRoute(nameof(FrutaDetallePage), typeof(FrutaDetallePage));
		Routing.RegisterRoute("DetalleFrutaPage", typeof(FrutaDetallePage));
	}
}
